using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerTPS : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 10f;
    public float jumpHeight = 1.2f;  

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
        if (context.performed)
        {
            isJumpPressed = true;
        }

        if (context.canceled)
        {
            isJumpPressed = false;
        }
    }

    private void HandleMovement()
    {
        if (controller.isGrounded)
        {
            if (yVelocity < 0)
            {
                yVelocity = -2f;
                animator.SetBool("IsJumping", false);
            }


            if (isJumpPressed)
            {
                yVelocity = Mathf.Sqrt(jumpHeight * -2f * -9.81f);
                animator.SetBool("IsJumping",true);
            }
        }
        else
        {
            yVelocity += -9.81f * Time.deltaTime;
        }

        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 moveDirection = camRight * moveInput.x + camForward * moveInput.y;

        Vector3 velocity = moveDirection * speed;
        velocity.y = yVelocity;

        controller.Move(velocity * Time.deltaTime);

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        float currentSpeed = new Vector2(moveInput.x, moveInput.y).magnitude;


        if (moveDirection != Vector3.zero)
        {

            animator.SetBool("IsWalking", true);
        }
        else
        {

            animator.SetBool("IsWalking", false);
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