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

    [Header("Foot IK Settings")]
    public bool enableFootIK = true;
    [Range(0, 2f)] public float raycastDistance = 1.0f;
    public LayerMask environmentLayer = ~0; // Default to all layers
    public float footOffset = 0.1f;

    [Range(0, 1f)] public float footIKWeight = 1.0f;

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator == null) return;

        if (enableFootIK)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, footIKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, footIKWeight);
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, footIKWeight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, footIKWeight);

            ProcessFootIK(AvatarIKGoal.LeftFoot);
            ProcessFootIK(AvatarIKGoal.RightFoot);
        }
        else
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0f);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0f);
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0f);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0f);
        }
    }

    private void ProcessFootIK(AvatarIKGoal footGoal)
    {
        Vector3 footPosition = animator.GetIKPosition(footGoal);
        Quaternion footRotation = animator.GetIKRotation(footGoal);
        Vector3 rootPosition = animator.rootPosition;

        // Calculate how high the foot is currently lifted in the animation
        float footAnimHeight = footPosition.y - rootPosition.y;

        // Raycast from above the character's root position, at the foot's X/Z location
        Vector3 rayOrigin = new Vector3(footPosition.x, rootPosition.y + raycastDistance, footPosition.z);

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, raycastDistance * 2f, environmentLayer, QueryTriggerInteraction.Ignore))
        {
            // Calculate new position: Ground height + animation height + offset
            Vector3 newPosition = footPosition;
            newPosition.y = hit.point.y + footAnimHeight + footOffset;
            animator.SetIKPosition(footGoal, newPosition);

            // Rotation (align to slope)
            Vector3 forward = footRotation * Vector3.forward;
            Vector3 projectedForward = Vector3.ProjectOnPlane(forward, hit.normal).normalized;
            if (projectedForward != Vector3.zero)
            {
                Quaternion newRotation = Quaternion.LookRotation(projectedForward, hit.normal);
                animator.SetIKRotation(footGoal, newRotation);
            }
        }
        else
        {
            // If we don't hit the ground (e.g. falling), keep the default animated position
            animator.SetIKPosition(footGoal, footPosition);
            animator.SetIKRotation(footGoal, footRotation);
        }
    }
}
