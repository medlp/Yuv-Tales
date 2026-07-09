using System.Collections;
using DS;
using UnityEngine;

/// <summary>
/// Mur invisible : empêche le joueur de sortir du périmètre défini par la Capsule Collider
/// (non-trigger, utilisée uniquement comme référence de rayon) tant qu'il n'a pas Has_Bag.
///
/// À placer sur le même GameObject que la Capsule Collider (rayon = périmètre)
/// et le DialogueTrigger (container "Bag", avec le Conditional Override
/// TryToEscape -> DoNotHaveBag déjà configuré, et "Lock Player Movement" coché).
///
/// Comportement :
/// - En LateUpdate (donc après le Move() de PlayerControllerTPS), si le joueur a dépassé
///   le rayon horizontalement, on le replace exactement sur le cercle limite.
///   La composante Y n'est pas touchée : s'il est en l'air, la gravité gérée par
///   PlayerControllerTPS continue de s'appliquer normalement, il retombe simplement
///   "collé" au mur au lieu de continuer à s'éloigner.
/// - La première fois qu'il touche le mur (par tentative), on passe TryToEscape à true
///   et on lance le dialogue -> le Conditional Override du DialogueTrigger bascule
///   vers DoNotHaveBag, et "Lock Player Movement" (déjà coché sur le DialogueTrigger)
///   coupe les déplacements via SetMovementLocked pendant le dialogue.
/// - Quand le dialogue se termine (DialogueManager.isActive repasse à false),
///   TryToEscape repasse à false.
/// </summary>
[RequireComponent(typeof(CapsuleCollider))]
public class BagEscapeGuard : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueTrigger dialogueTrigger;
    [SerializeField] private PlayerControllerTPS playerController;

    [Header("Parametres")]
    [SerializeField] private string hasNecklaceFlag = "Has_Necklace";
    [SerializeField] private string tryToEscapeFlag = "TryToEscape";

    private CapsuleCollider perimeterCollider;
    private Transform player;
    private bool hasTriggeredThisAttempt;
    private bool waitingForDialogueEnd;

    private void Awake()
    {
        perimeterCollider = GetComponent<CapsuleCollider>();

        if (dialogueTrigger == null)
            dialogueTrigger = GetComponent<DialogueTrigger>();

        if (player == null)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                if (playerController == null)
                    playerController = playerObj.GetComponent<PlayerControllerTPS>();
            }
        }
        else if (playerController == null)
        {
            playerController = player.GetComponent<PlayerControllerTPS>();
        }
    }

    private void LateUpdate()
    {
        if (player == null) return;

        // Plus aucune restriction une fois le sac obtenu
        if (DSDialogueFlags.Get(hasNecklaceFlag))
        {
            hasTriggeredThisAttempt = false;
            return;
        }

        float radius = perimeterCollider.radius * transform.lossyScale.x;

        Vector3 offset = player.position - transform.position;
        Vector3 flatOffset = new Vector3(offset.x, 0f, offset.z);

        if (flatOffset.magnitude > radius)
        {
            // Mur invisible : on ramene le joueur pile sur le cercle limite,
            // la composante verticale n'est pas touchee (la gravite continue de jouer normalement)
            Vector3 clampedFlat = flatOffset.normalized * radius;
            player.position = transform.position + new Vector3(clampedFlat.x, offset.y, clampedFlat.z);

            TryTriggerEscapeDialogue();
        }
        else
        {
            hasTriggeredThisAttempt = false;
        }
    }

    private void TryTriggerEscapeDialogue()
    {
        if (hasTriggeredThisAttempt || DialogueManager.isActive) return;
        hasTriggeredThisAttempt = true;


        DSDialogueFlags.Set(tryToEscapeFlag, true);

        playerController.LockCamera(true);
        playerController.SetMovementLocked(true);
        dialogueTrigger.StartDialogue();


        if (!waitingForDialogueEnd)
            StartCoroutine(ResetFlagWhenDialogueEnds());


    }

    private IEnumerator ResetFlagWhenDialogueEnds()
    {
        waitingForDialogueEnd = true;

        yield return null; // laisse le temps a OpenDSDialogue de passer isActive a true
        yield return new WaitUntil(() => !DialogueManager.isActive);

        DSDialogueFlags.Set(tryToEscapeFlag, false);
        waitingForDialogueEnd = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        var cc = GetComponent<CapsuleCollider>();
        if (cc == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, cc.radius * transform.lossyScale.x);
    }
#endif
}