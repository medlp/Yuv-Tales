using System.Collections;
using DS;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class InterpellationZone : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueTrigger dialogueTrigger;
    [SerializeField] private PlayerControllerTPS playerController;

    [Header("Parametres")]
    [SerializeField] private string approachDialogueName = "Approach";
    [SerializeField] private string repeatDialogueName = "Dialogue1";
    [SerializeField] private string playerTag = "Player";

    private string HasApproachedFlag => "HasApproached";

    private bool playerInZone;

    private void Awake()
    {
        if (dialogueTrigger == null)
            dialogueTrigger = GetComponentInParent<DialogueTrigger>();

        if (playerController == null)
        {
            var playerObj = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObj != null)
                playerController = playerObj.GetComponent<PlayerControllerTPS>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerInZone) return;
        if (!other.CompareTag(playerTag)) return;
        if (DialogueManager.isActive) return;

        playerInZone = true;

        bool isFirstApproach = !DSDialogueFlags.Get(HasApproachedFlag);

        if (isFirstApproach)
        {
            DSDialogueFlags.Set(HasApproachedFlag, true);

            if (playerController != null)
            {
                playerController.LockCamera(true);
                playerController.SetMovementLocked(true);

                // --- NOUVEAUTÉ : ORIENTATION VERS LE PNJ ---
                // Calcule la direction vers le PNJ (ce script étant sur un enfant ou sur le PNJ, on vise le parent/pivot)
                Vector3 directionToPNJ = transform.position - playerController.transform.position;
                directionToPNJ.y = 0f; // On ignore la hauteur pour éviter que le joueur ne penche

                if (directionToPNJ != Vector3.zero)
                {
                    // Aligne instantanément le joueur vers le PNJ
                    playerController.transform.rotation = Quaternion.LookRotation(directionToPNJ);
                }

                // Recentre immédiatement la caméra Cinemachine juste derrière le dos du joueur (qui regarde maintenant le PNJ)
                playerController.SmoothCameraBehindPlayer(0.2f);
                // -------------------------------------------
            }

            if (dialogueTrigger != null)
            {
                dialogueTrigger.startingDialogueName = approachDialogueName;
                dialogueTrigger.StartDialogue();
                dialogueTrigger.startingDialogueName = repeatDialogueName;
            }

            StartCoroutine(UnlockPlayerWhenDialogueEnds());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
            playerInZone = false;
    }

    private IEnumerator UnlockPlayerWhenDialogueEnds()
    {
        yield return null;
        yield return new WaitUntil(() => !DialogueManager.isActive);

        if (playerController != null)
        {
            playerController.LockCamera(false);
            playerController.SetMovementLocked(false);
        }
    }
}