using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
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
    public float slideInitialSpeed = 18f;
    public float slideDeceleration = 4f;
    public float minSpeedToSlide = 10f;

    [Header("Jump Settings")]
    public float jumpHeight = 1.2f;
    public float jumpCooldown = 0.5f;
    private float jumpTimer = 0f;

    [Header("Crouch Settings")]
    public float crouchSpeed = 2.5f;
    private float crouchHeight = 1f;
    private float crouchRadius = 0.5f;
    private Vector3 crouchCenter = new Vector3(0, 0.55f, 0);

    // Variables privées
    private bool isSprinting;
    [SerializeField] private bool isCrouching = false;
    [SerializeField] private bool isSliding = false;
    private float originalHeight;
    private float originalRadius;
    private Vector3 originalCenter;

    private CharacterController controller;
    private Vector2 moveInput;
    private float yVelocity;
    private Transform cameraTransform;
    private bool isJumpPressed;

    [SerializeField] private Vector3 currentVelocityXZ;

    Animator animator;

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

        controller.height = 1.8f;
        controller.center = Vector3.up;
        controller.radius = 0.5f;

        originalHeight = controller.height;
        originalCenter = controller.center;
        originalRadius = controller.radius;
    }

    void Update()
    { 
        HandleMovement();
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isJumpPressed = true;
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isSprinting = true;
        }

        if (context.canceled)
        {
            isSprinting = false;
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (currentVelocityXZ.magnitude >= minSpeedToSlide && isCrouching == false)
            {
                isSliding = true;
                isCrouching = true;
                isSprinting = false;

                Vector3 slideDirection = currentVelocityXZ.normalized;

                if (slideDirection == Vector3.zero)
                {
                    slideDirection = transform.forward;
                }

                currentVelocityXZ = slideDirection * slideInitialSpeed;

                animator.SetBool("IsSliding", true);
                animator.SetBool("IsCrouching", false);

                controller.height = crouchHeight;
                controller.radius = crouchRadius;
                controller.center = crouchCenter;
                return;
            }

            if (isCrouching || isSliding)
            {

                if (CanStandUp() == false)
                {
                    return;
                }

                isSliding = false;
                isCrouching = false;

                animator.SetBool("IsSliding", false);
                animator.SetBool("IsCrouching", false);

                controller.height = originalHeight;
                controller.radius = originalRadius;
                controller.center = originalCenter;
            }
            else
            {
                isCrouching = true;
                animator.SetBool("IsCrouching", true);

                controller.height = crouchHeight;
                controller.radius = crouchRadius;
                controller.center = crouchCenter;
            }
        }
    }

    private void HandleMovement()
    {
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 inputDirection = camRight * moveInput.x + camForward * moveInput.y;
        float speedTarget = 0f;
        Vector3 targetVelocityXZ = Vector3.zero;

        if (isSliding)
        {
            targetVelocityXZ = Vector3.zero;

            if (currentVelocityXZ.magnitude <= 0.5f)
            {
                isSliding = false;
                animator.SetBool("IsSliding", false);
                currentVelocityXZ = Vector3.zero;

                if (isCrouching)
                {
                    animator.SetBool("IsCrouching", true);
                }
                else
                {
                    animator.SetBool("IsCrouching", false);
                }
            }
        }
        else
        {
            if (inputDirection != Vector3.zero)
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

            targetVelocityXZ = inputDirection * speedTarget;
        }

        if (isSliding)
        {
            currentVelocityXZ = Vector3.MoveTowards(currentVelocityXZ, targetVelocityXZ, slideDeceleration * Time.deltaTime);
        }

        if (controller.isGrounded)
        {
            animator.SetBool("IsGrounded", true);

            if (yVelocity < 0)
            {
                yVelocity = -5f;
            }

            if (jumpTimer > 0f)
            {
                jumpTimer -= Time.deltaTime;
            }

            if (isSliding == false)
            {
                if (inputDirection != Vector3.zero)
                {
                    float currentSpeed = currentVelocityXZ.magnitude;
                    float targetSpeed = targetVelocityXZ.magnitude;

                    currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
                    currentVelocityXZ = inputDirection.normalized * currentSpeed;
                }
                else
                {
                    currentVelocityXZ = Vector3.MoveTowards(currentVelocityXZ, Vector3.zero, deceleration * Time.deltaTime);
                }
            }

            if (isJumpPressed)
            {
                if (isCrouching == false)
                {
                    if (isSliding == false)
                    {
                        if (jumpTimer <= 0f)
                        {
                            yVelocity = Mathf.Sqrt(jumpHeight * -2f * -9.81f);
                            animator.SetTrigger("Jump");
                            jumpTimer = jumpCooldown;
                        }
                    }
                }

                isJumpPressed = false;
            }
        }
        else
        {
            animator.SetBool("IsGrounded", false);
            yVelocity += -9.81f * Time.deltaTime;

            if (isSliding == false)
            {
                if (inputDirection != Vector3.zero)
                {
                    float currentSpeed = currentVelocityXZ.magnitude;
                    float targetSpeed = targetVelocityXZ.magnitude;

                    currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, (acceleration * 0.5f) * Time.deltaTime);
                    currentVelocityXZ = inputDirection.normalized * currentSpeed;
                }
            }
        }

        Vector3 finalVelocity = currentVelocityXZ;
        finalVelocity.y = yVelocity;

        controller.Move(finalVelocity * Time.deltaTime);

        if (inputDirection != Vector3.zero)
        {
            if (isSliding == false)
            {
                Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        float currentSpeedForAnimator = currentVelocityXZ.magnitude;

        if (isSliding)
        {
            currentSpeedForAnimator = 0f;
        }

        animator.SetFloat("Speed", currentSpeedForAnimator, 0.2f, Time.deltaTime);
    }

    private bool CanStandUp()
    {
        Vector3 startPoint = transform.position + crouchCenter;
        float distanceToCheck = originalHeight - crouchHeight;

        float checkRadius = originalRadius * 0.9f;

        if (Physics.SphereCast(startPoint, checkRadius, Vector3.up, out RaycastHit hit, distanceToCheck, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            return false;
        }
        else
        {
            return true;
        }
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