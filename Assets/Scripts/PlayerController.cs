using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerTPS : MonoBehaviour
{

    public float walkSpeed = 5f;
    public float sprintSpeed = 15.0f;
    public float rotationSpeed = 10f;
    public float jumpHeight = 1.2f;
    public float currentSpeed = 5f;


    private bool isSprinting;

    private CharacterController controller;
    private Vector2 moveInput;
    private float yVelocity;
    private Transform cameraTransform;
    private bool isJumpPressed;

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
        Debug.Log("pressed");

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

    private void HandleMovement()
    {

        if (controller.isGrounded)
        {
            animator.SetBool("IsGrounded", true); 

            if (yVelocity < 0)
            {
                yVelocity = -5f;
            }

            if (isJumpPressed)
            {
                yVelocity = Mathf.Sqrt(jumpHeight * -2f * -9.81f);
                animator.SetTrigger("Jump");
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


        float currentSpeed = 0f;
        if (moveDirection != Vector3.zero)
        {
            if (isSprinting)
            {
                currentSpeed = sprintSpeed; 
            }
            else
            {
                currentSpeed = walkSpeed;
            }
        }


        Vector3 velocity = moveDirection * currentSpeed;
        velocity.y = yVelocity;

        controller.Move(velocity * Time.deltaTime);


        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }


        animator.SetFloat("Speed", currentSpeed, 0.2f, Time.deltaTime);
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