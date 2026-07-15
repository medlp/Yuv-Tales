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
                Vector3 directionToPNJ = transform.parent.position - playerController.transform.position;
                directionToPNJ.y = 0f;
                if (directionToPNJ != Vector3.zero)
                {
                    playerController.transform.rotation = Quaternion.LookRotation(directionToPNJ);
                }
                playerController.SmoothCameraBehindPlayer(0.15f);
            }

            if (dialogueTrigger != null)
            {
                dialogueTrigger.startingDialogueName = approachDialogueName;
                dialogueTrigger.StartDialogue(); 
                dialogueTrigger.startingDialogueName = repeatDialogueName;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
            playerInZone = false;
    }
}