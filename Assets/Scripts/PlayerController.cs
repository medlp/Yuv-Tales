using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerTPS : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 15.0f;
    public float rotationSpeed = 10f;
    public float currentSpeed = 5f;

    [Header("Jump Settings")]
    public float jumpHeight = 1.2f;
    public float jumpCooldown = 1.0f; 
    private float jumpTimer = 0f; 

    [Header("Crouch Settings")]
    public float crouchSpeed = 2.5f;
    private float crouchHeight = 1.0f;
    private Vector3 crouchCenter = new Vector3(0, 0.5f, 0);

    private StaminaSystem staminaSystem;

    private bool isSprinting;
    private bool isCrouching = false;
    private float originalHeight;
    private Vector3 originalCenter;

    private CharacterController controller;
    private Vector2 moveInput;
    private float yVelocity;
    private Transform cameraTransform;
    private bool isJumpPressed;

    Animator animator;

    private DialogueTrigger currentDialogTrigger;

    private CinemachineInputAxisController cinemachineInputAxisController; 


    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        staminaSystem = GetComponent<StaminaSystem>();

        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
            cinemachineInputAxisController = FindFirstObjectByType<CinemachineInputAxisController>();

        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        originalHeight = controller.height;
        originalCenter = controller.center;
    }

    void Update()
    {
        if (jumpTimer > 0f)
        {
            jumpTimer -= Time.deltaTime;
        }

        // POUR BLOQUER LE MOUVEMENT DU JOUEUR PENDANT UN DIALOGUE
        if (DialogueManager.isActive && IsUsingGamepad())
        {
            animator.SetFloat("Speed", 0f, 0.2f, Time.deltaTime);
            return;
        }

        HandleMovement();

        staminaSystem.OnUpdate(isSprinting);
    }

    private bool IsUsingGamepad()
    {
        return GetComponent<PlayerInput>().currentControlScheme == "Gamepad";
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(DialogueManager.isActive)
            return;
        

        if (context.performed)
        {
            isJumpPressed = true;
        }

        if (context.canceled)
        {
            isJumpPressed = false;
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed && moveInput != Vector2.zero && !staminaSystem.isEmpty)
        {
            isSprinting = true;
        }

        if (context.canceled || staminaSystem.isEmpty)
        {
            isSprinting = false;
            staminaSystem.isRecovering = true;
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isCrouching)
            {
                isCrouching = false;
            }
            else
            {
                isCrouching = true;
            }

            if (isCrouching)
            {
                controller.height = crouchHeight;
                controller.center = crouchCenter;
            }
            else
            {
                controller.height = originalHeight;
                controller.center = originalCenter;
            }

            animator.SetBool("IsCrouching", isCrouching);
        }
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
        if (context.started)
        {
            Vector2 input = context.ReadValue<Vector2>();
            FindFirstObjectByType<DialogueManager>().NavigateChoices(input);
        }
    }

    public void OnConfirmChoice(InputAction.CallbackContext context)
    {         
        if(!DialogueManager.isActive)
            return;

        if (context.performed)
        {                

                FindFirstObjectByType<DialogueManager>().ConfirmChoice();
            
        }
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

    public void LockCamera(bool lockIt)
    {
        if (cinemachineInputAxisController != null)
            cinemachineInputAxisController.enabled = !lockIt;

        ToggleCursorState();
    }

    private void HandleMovement()
    {
        if (controller.isGrounded)
        {
            animator.SetBool("IsGrounded", true);

            if (yVelocity < 0)
            {
                yVelocity = -5f;
            }

            if (isJumpPressed && isCrouching == false)
            {
                if (jumpTimer <= 0f)
                {
                    yVelocity = Mathf.Sqrt(jumpHeight * -2f * -9.81f);
                    animator.SetTrigger("Jump");

                    jumpTimer = jumpCooldown;
                }

                isJumpPressed = false;
            }
        }
        else
        {
            animator.SetBool("IsGrounded", false);
            yVelocity += -9.81f * Time.deltaTime;
        }

        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 moveDirection = camRight * moveInput.x + camForward * moveInput.y;

        float speedTarget = 0f;
        if (moveDirection != Vector3.zero)
        {
            if (isCrouching)
            {
                speedTarget = crouchSpeed;
            }
            else if (isSprinting && !staminaSystem.isEmpty)
            {
                speedTarget = sprintSpeed;
            }
            else if (staminaSystem.isEmpty)
            {
                speedTarget = walkSpeed;
            }
            else
            {
                speedTarget = walkSpeed;
            }
        }

        Vector3 velocity = moveDirection * speedTarget;
        velocity.y = yVelocity;

        controller.Move(velocity * Time.deltaTime);

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        animator.SetFloat("Speed", speedTarget, 0.2f, Time.deltaTime);
    }

    public void ToggleCursorState()
    {
        if (Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (Cursor.visible == true)
        {
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
        }
    }
}