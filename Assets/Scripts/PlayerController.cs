using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerTPS : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 7f;
    public float sprintSpeed = 16.0f;
    public float rotationSpeed = 10f;
    

    [Header("Momentum Settings")]
    public float acceleration = 4f;   
    public float deceleration = 4f; 

    [Header("Jump Settings")]
    public float jumpHeight = 1.2f;
    public float jumpCooldown = 1.0f;
    private float jumpTimer = 0f;

    [Header("Crouch Settings")]
    public float crouchSpeed = 2.5f;
    private float crouchHeight = 1.0f;
    private Vector3 crouchCenter = new Vector3(0, 0.5f, 0);
     
    private bool isSprinting;
    private bool isCrouching = false;
    private float originalHeight;
    private Vector3 originalCenter;

    private CharacterController controller;
    private Vector2 moveInput;
    private float yVelocity;
    private Transform cameraTransform;
    private bool isJumpPressed;
     

    private float speedTarget = 0f;
    private Vector3 currentVelocityXZ;

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

        originalHeight = controller.height;
        originalCenter = controller.center;
    }

    void Update()
    {
        if (jumpTimer > 0f)
        {
            jumpTimer -= Time.deltaTime;
        }

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

        if (context.canceled)
        {
            isJumpPressed = false;
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

    private void HandleMovement()
    { 
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 inputDirection = camRight * moveInput.x + camForward * moveInput.y;
         
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
         
        Vector3 targetVelocityXZ = inputDirection * speedTarget;
         
        if (controller.isGrounded)
        {
            animator.SetBool("IsGrounded", true);

            if (yVelocity < 0)
            {
                yVelocity = -5f;
            }
             
            if (inputDirection != Vector3.zero)
            { 
                currentVelocityXZ = Vector3.Lerp(currentVelocityXZ, targetVelocityXZ, acceleration * Time.deltaTime);
            }
            else
            { 
                currentVelocityXZ = Vector3.Lerp(currentVelocityXZ, Vector3.zero, deceleration * Time.deltaTime);
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
         
        Vector3 finalVelocity = currentVelocityXZ;
        finalVelocity.y = yVelocity;

        controller.Move(finalVelocity * Time.deltaTime);
         
        if (inputDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
         
        float currentSpeedForAnimator = currentVelocityXZ.magnitude;
        animator.SetFloat("Speed", currentSpeedForAnimator, 0.2f, Time.deltaTime);
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