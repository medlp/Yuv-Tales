using UnityEngine;


namespace DS
{
    using ScriptableObjects;

    public class DSDialogue : MonoBehaviour
    {
        [SerializeField] private DSDialogueContainerSO dialogueContainer;
        [SerializeField] private DSDialogueGroupSO dialogueGroup;
        [SerializeField] private DSDialogueSO dialogue;


        [SerializeField] private bool groupedDialogues;
        [SerializeField] private bool startingDialoguesOnly;
    }
}

