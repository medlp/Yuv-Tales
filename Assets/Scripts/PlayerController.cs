using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

    [Header("Stamina")]
    [SerializeField ] private float maxStamina = 100.0f;
    private float minStamina;
    private float currentStamina;
    [SerializeField] private Image staminaImage;
    private bool isRecovering = false;
    private bool isEmpty = false;

    [Header("Health")]
    [SerializeField] private float maxHealth = 50.0f;
    private float minHealth;
    private float currentHealth;
    [SerializeField] private Image HealthImage;


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

    private DialogTrigger currentDialogTrigger;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        originalHeight = controller.height;
        originalCenter = controller.center;

        currentStamina = maxStamina; minStamina = (maxStamina * 0.16f);
        currentHealth = maxHealth; minHealth = (maxHealth * 0.136f);

    }

    void Update()
    {

        if (jumpTimer > 0f)
        {
            jumpTimer -= Time.deltaTime;
        }

        HandleMovement();

        //endurance used without moving
        //recover on first empty
        //stop sprinting on empty
        if (isSprinting)
        {
            if (currentStamina > minStamina)
            {
                currentStamina -= 10.0f * Time.deltaTime;
            }
            else if (currentStamina < maxStamina)
            {
                if (currentStamina <= minStamina)
                    isEmpty = true;

                isRecovering = true;
            }
        }
        else if (currentStamina < maxStamina)
        {
            if (isEmpty)
            {
                currentStamina += 10.0f * Time.deltaTime;
                isEmpty = false;
            }
            else if (isRecovering)
            {
                currentStamina += 10.0f * Time.deltaTime;
            }
        }

        staminaImage.fillAmount = (currentStamina / maxStamina);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && !isEmpty)
        {
            isJumpPressed = true;
        }

        if (context.canceled || isEmpty)
        {
            isJumpPressed = false;
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed && yVelocity != 0.0f)
        {
            isSprinting = true;
        }

        if (context.canceled || isEmpty)
        {
            isSprinting = false;
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
        if (context.performed)
        {
            if (DialogManager.isActive)
                FindFirstObjectByType<DialogManager>().NextMessage();
            else if (currentDialogTrigger != null)
                currentDialogTrigger.StartDialogue();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<DialogTrigger>(out DialogTrigger trigger))
            currentDialogTrigger = trigger;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<DialogTrigger>(out DialogTrigger trigger))
            currentDialogTrigger = null;
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
            else if (isSprinting)
            {
                speedTarget = sprintSpeed;
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

    public void SetCursorState()
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