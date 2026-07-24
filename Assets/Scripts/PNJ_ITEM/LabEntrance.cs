using UnityEngine;
using DS;

[RequireComponent(typeof(DialogueTrigger))]
public class LabEntrance : MonoBehaviour
{
    [Header("Flag Condition")]
    [Tooltip("Le dialogue se declenche si ce flag DS vaut false")]
    [SerializeField] private string requiredFlag = "GetPermission";

    [Header("Access Collider")]
    [Tooltip("Collider physique (non-trigger) qui bloque le passage tant que le flag n'est pas obtenu.")]
    [SerializeField] private Collider blockingCollider;

    private DialogueTrigger dialogueTrigger;

    private void Start()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
        UpdateBlockingCollider();
    }

    private void Update()
    {
        UpdateBlockingCollider();
    }

    private void UpdateBlockingCollider()
    {
        if (blockingCollider == null) return;

        bool hasPermission = DSDialogueFlags.Get(requiredFlag);
        if (blockingCollider.enabled == hasPermission)
            blockingCollider.enabled = !hasPermission;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (DialogueManager.isActive)
            return;

        if (DSDialogueFlags.Get(requiredFlag))
            return; // Le joueur a deja la permission, on ne bloque pas l'entree

        dialogueTrigger.StartDialogue();
    }
}