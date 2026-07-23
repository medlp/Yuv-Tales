using UnityEngine;
using DS;

[RequireComponent(typeof(DialogueTrigger))]
public class LabEntrance : MonoBehaviour
{
    [Header("Flag Condition")]
    [Tooltip("Le dialogue se declenche si ce flag DS vaut false")]
    [SerializeField] private string requiredFlag = "GetPermission";

    private DialogueTrigger dialogueTrigger;

    private void Start()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
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