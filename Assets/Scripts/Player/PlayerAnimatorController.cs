using UnityEngine;

/// <summary>
/// Gère toutes les interactions avec l'Animator du joueur.
/// Doit être placé sur le même GameObject que PlayerControllerTPS.
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetSpeed(float speed, float dampTime = 0.2f)
    {
        animator.SetFloat("Speed", speed, dampTime, Time.deltaTime);
    }

    public void SetGrounded(bool grounded)
    {
        animator.SetBool("IsGrounded", grounded);
    }

    public void SetCrouching(bool crouching)
    {
        animator.SetBool("IsCrouching", crouching);
    }

    public void SetSliding(bool sliding)
    {
        animator.SetBool("IsSliding", sliding);
    }

    public void TriggerJump()
    {
        animator.SetTrigger("Jump");
    }
}
