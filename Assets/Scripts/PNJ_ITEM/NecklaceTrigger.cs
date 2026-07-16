using System.Collections;
using DS;
using UnityEngine;


[RequireComponent(typeof(CapsuleCollider))]
public class NecklaceEscape : MonoBehaviour
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

        if (DSDialogueFlags.Get(hasNecklaceFlag))
        {
            hasTriggeredThisAttempt = false;
            gameObject.SetActive(false);
            Debug.Log("DESACTIVE : Flag possédé");
            return;
        }

        float radius = perimeterCollider.radius * transform.lossyScale.x;

        Vector3 offset = player.position - transform.position;
        Vector3 flatOffset = new Vector3(offset.x, 0f, offset.z);

        if (flatOffset.magnitude > radius)
        {
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

        dialogueTrigger.StartDialogue();


        if (!waitingForDialogueEnd)
            StartCoroutine(ResetFlagWhenDialogueEnds());
    }

    private IEnumerator ResetFlagWhenDialogueEnds()
    {
        waitingForDialogueEnd = true;

        yield return null;
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