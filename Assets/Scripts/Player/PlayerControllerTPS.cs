using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Gère uniquement le mouvement du joueur (marche, sprint, saut, accroupissement, slide).
/// Délègue la caméra à PlayerCameraController et les animations à PlayerAnimatorController.
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerCameraController))]
public class PlayerControllerTPS : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 15.0f;
    public float rotationSpeed = 15f;

    [Header("Momentum Settings")]
    public float acceleration = 15f;
    public float deceleration = 20f;

    [Header("Slide Settings")]
    public float slideSpeedBoost = 5f;
    public float slideDeceleration = 4f;
    public float minSpeedToSlide = 10f;

    [Header("Jump Settings")]
    public float jumpHeight = 1.2f;
    public float jumpCooldown = 0.5f;

    [Header("Crouch Settings")]
    public float crouchSpeed = 2.5f;

    // ── Inspector debug ──────────────────────────────────────────────────────
    [SerializeField] private bool isCrouching;
    [SerializeField] private bool isSliding;
    [SerializeField] private Vector3 currentVelocityXZ;

    // ── Collider presets ─────────────────────────────────────────────────────
    private readonly float crouchHeight = 1f;
    private readonly float crouchRadius = 0.25f;
    private readonly Vector3 crouchCenter = new Vector3(0, 0.55f, 0);
    private float originalHeight;
    private float originalRadius;
    private Vector3 originalCenter;

    // ── Refs ──────────────────────────────────────────────────────────────────
    private CharacterController controller;
    private PlayerCameraController cameraController;
    private PlayerAnimatorController animController;
    private Transform cameraTransform;
    private InventoryUI inventoryUI;
    private CinemachineInputAxisController cinemachineInputAxisController;
    private DialogueTrigger currentDialogTrigger;
    private StaminaSystem staminaSystem;
    private FamiliarPingController pingController;

    // ── State ─────────────────────────────────────────────────────────────────
    private Vector2 moveInput;
    private float yVelocity;
    private float jumpTimer;
    private bool isJumpPressed;
    private bool isSprinting;
    private bool isLockCamera;
    private bool isMovementLocked;

    // ── Const ───────────────────────────────────────────────────────────────────────
    private const float Gravity = 9.81f;

    // ────────────────────────────────────────────────────────────────────────────────

    #region Unity Methods
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        cameraController = GetComponent<PlayerCameraController>();
        animController = GetComponentInChildren<PlayerAnimatorController>();
        inventoryUI = FindFirstObjectByType<InventoryUI>();
        staminaSystem = GetComponent<StaminaSystem>();
        pingController = GetComponent<FamiliarPingController>();

        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
            cinemachineInputAxisController = FindFirstObjectByType<CinemachineInputAxisController>();
        }
    }

    void Start()
    {
        var playerInput = GetComponent<PlayerInput>();
        playerInput.actions.Disable();
        playerInput.SwitchCurrentActionMap("Player");
         

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        controller.height = 1.8f;
        controller.center = Vector3.up;
        controller.radius = 0.25f;

        originalHeight = controller.height;
        originalCenter = controller.center;
        originalRadius = controller.radius;
    }

    void Update()
    {
        HandleMovement();

        ////////////////////////////////////////
        //its here and a bit ugly but its work at least
        ////////////////////////////////////////

        if (staminaSystem.isEmpty)
        {
            isSprinting = false;
            staminaSystem.isRecovering = true;
        }

        bool isActuallyMoving = moveInput != Vector2.zero;

        staminaSystem.OnUpdate(isSprinting && isActuallyMoving, isCrouching);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<DialogueTrigger>(out DialogueTrigger trigger))
            currentDialogTrigger = trigger;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<DialogueTrigger>(out DialogueTrigger trigger))
        {
            currentDialogTrigger = null;
        }
    }

    #endregion

    #region Controller Methods
    public void OnMove(InputAction.CallbackContext context)
    {
        if (isMovementLocked) return;
        moveInput = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed) isJumpPressed = true;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed && moveInput != Vector2.zero && !staminaSystem.isEmpty)
            isSprinting = true;

        if (context.canceled || staminaSystem.isEmpty)
        {
            isSprinting = false;
            staminaSystem.isRecovering = true;
        }
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        if (context.performed)
            cameraController.ToggleZoom(isCrouching);
    }

    public void OnReset(InputAction.CallbackContext context)
    {
        if (context.performed)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (PauseManager.Instance != null)
            {
                PauseManager.Instance.TogglePause();
            }
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (!context.performed || cameraController.IsZooming) return;
         
        if (currentVelocityXZ.magnitude >= minSpeedToSlide && !isCrouching)
        {
            StartSlide();
            return;
        }
         
        if (isCrouching || isSliding)
        {
            if (!CanStandUp()) return;
            StandUp();
        }
        else
        {
            Crouch();
        }
    }
    public void OnToggleInventory(InputAction.CallbackContext context)
    {
        inventoryUI.OnToggleInventory(context);
        if (isLockCamera)
            LockCamera(false);
        else
            LockCamera(true);
    }

    public void OnFocus(InputAction.CallbackContext context)
    {
        if (context.performed)
            SmoothCameraBehindPlayer();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (currentDialogTrigger != null && !DialogueManager.isActive)
        {
            LockCamera(true);
            currentDialogTrigger.StartDialogue();
        }
    }

    public void OnNavigateChoices(InputAction.CallbackContext context)
    {
        if (!DialogueManager.isActive)
            return;

        if (context.performed)
        {
            Vector2 input = context.ReadValue<Vector2>();
            FindFirstObjectByType<DialogueManager>().NavigateChoices(input);
        }
    }

    public void OnConfirmChoice(InputAction.CallbackContext context)
    {
        if (!DialogueManager.isActive)
            return;

        if (context.performed)
        {
            FindFirstObjectByType<DialogueManager>().ConfirmChoice();
        }
    }

    public void OnPing(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            pingController.OnPingPerformed(context);
        }
    }
    #endregion

    #region Utility Methods
    public void SetMovementLocked(bool locked)
    {
        isMovementLocked = locked;
        if (locked)
        {
            moveInput = Vector2.zero;
            currentVelocityXZ = Vector3.zero;
        }
    }

    public void LockCamera(bool lockIt)
    {
        if (cinemachineInputAxisController != null)
            cinemachineInputAxisController.enabled = !lockIt;

        isLockCamera = lockIt;
    }

    public void SetCursorActive(bool active)
    {
        if(active)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else 
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void SnapCameraBehindPlayer()
    {
        cameraController.SnapFocusBehindPlayer();
    }

    public void SmoothCameraBehindPlayer(float time = 0.2f)
    {
        cameraController.FocusBehindPlayer(time);
    }

    #endregion 

    private void StartSlide()
    {
        isSliding = true;
        isCrouching = true;
        isSprinting = false;

        Vector3 dir = currentVelocityXZ.magnitude > 0.01f ? currentVelocityXZ.normalized : transform.forward;
        currentVelocityXZ = dir * (currentVelocityXZ.magnitude + slideSpeedBoost);

        animController.SetSliding(true);
        animController.SetCrouching(false);
        SetColliderCrouch();
    }

    private void Crouch()
    {
        isCrouching = true;
        animController.SetCrouching(true);
        SetColliderCrouch();
    }

    private void StandUp()
    {
        isSliding = false;
        isCrouching = false;
        animController.SetSliding(false);
        animController.SetCrouching(false);
        SetColliderStand();
    }

    private void SetColliderCrouch()
    {
        controller.height = crouchHeight;
        controller.radius = crouchRadius;
        controller.center = crouchCenter;
    }

    private void SetColliderStand()
    {
        controller.height = originalHeight;
        controller.radius = originalRadius;
        controller.center = originalCenter;
    }

    private bool CanStandUp()
    {
        return !Physics.SphereCast(
            transform.position + crouchCenter,
            originalRadius * 0.9f,
            Vector3.up,
            out _,
            originalHeight - crouchHeight,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore);
    }
     
    private void HandleMovement()
    {
        Vector3 inputDirection = GetInputDirectionRelativeToCamera();

        UpdateVelocityXZ(inputDirection);
        UpdateVertical();
        ApplyMove();
        RotateTowardsInput(inputDirection);
        UpdateAnimator();
    }

    private Vector3 GetInputDirectionRelativeToCamera()
    {
        Vector3 camForward = cameraTransform.forward; camForward.y = 0f; camForward.Normalize();
        Vector3 camRight   = cameraTransform.right;   camRight.y   = 0f; camRight.Normalize();
        return camRight * moveInput.x + camForward * moveInput.y;
    }

    private void UpdateVelocityXZ(Vector3 inputDirection)
    {
        if (isSliding)
        {
            currentVelocityXZ = Vector3.MoveTowards(currentVelocityXZ, Vector3.zero, slideDeceleration * Time.deltaTime);

            if (currentVelocityXZ.magnitude <= 0.5f)
            {
                isSliding = false;
                currentVelocityXZ = Vector3.zero;
                animController.SetSliding(false);
                animController.SetCrouching(isCrouching);
            }
            return;
        }

        if (inputDirection != Vector3.zero)
        {
            float target = isCrouching ? crouchSpeed : isSprinting ? sprintSpeed : walkSpeed;
            float current = currentVelocityXZ.magnitude;
            current = Mathf.MoveTowards(current, target, acceleration * Time.deltaTime);
            currentVelocityXZ = inputDirection.normalized * current;
        }
        else
        {
            currentVelocityXZ = Vector3.MoveTowards(currentVelocityXZ, Vector3.zero, deceleration * Time.deltaTime);
        }
    }

    private void UpdateVertical()
    {
        if (controller.isGrounded)
        {
            animController.SetGrounded(true);

            if (yVelocity < 0) 
                yVelocity = -5f;

            if (jumpTimer > 0f) 
                jumpTimer -= Time.deltaTime;

            if (isJumpPressed && !isCrouching && !isSliding && jumpTimer <= 0f)
            {
                yVelocity = Mathf.Sqrt(jumpHeight * 2f * Gravity);
                animController.TriggerJump();
                jumpTimer = jumpCooldown;
            }

            isJumpPressed = false;
        }
        else
        {
            animController.SetGrounded(false);
            yVelocity -= Gravity * Time.deltaTime;
        }

    }

    private void ApplyMove()
    {
        Vector3 velocity = currentVelocityXZ;
        velocity.y = yVelocity;
        controller.Move(velocity * Time.deltaTime);
    }

    private void RotateTowardsInput(Vector3 inputDirection)
    {
        if (inputDirection == Vector3.zero || isSliding) return;
        Quaternion target = Quaternion.LookRotation(inputDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.deltaTime);
    }

    private void UpdateAnimator()
    {
        float speed = isSliding ? 0f : currentVelocityXZ.magnitude;
        animController.SetSpeed(speed);
    }

    #region Save System
    public Vector3 GetPosition()
    {
        return transform.position;
    }
    public float GetRotationY()
    {
        return transform.eulerAngles.y;
    }

    public void Warp(Vector3 position, float rotationY)
    {
        bool wasEnabled = controller.enabled;
        controller.enabled = false;

        transform.position = position;
        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);

        currentVelocityXZ = Vector3.zero;
        yVelocity = 0f;

        controller.enabled = wasEnabled;
    }
    #endregion
}
