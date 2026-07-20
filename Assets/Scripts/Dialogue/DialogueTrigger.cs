using UnityEngine;
using DS.ScriptableObjects;
using DS;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Graph")]
    public DSDialogueContainerSO dialogueContainer;
    public string startingDialogueName = "";
    public Actor actor;

    [Header("Dialogue Settings")]
    public bool lockPlayerMovement = false;
    public bool lockPlayerCamera = true;

    [Header("Conditional Overrides")]
    [Tooltip("Évaluées dans l'ordre — le premier override dont la condition est vraie remplace le dialogue de départ.")]
    public DSDialogueOverride[] dialogueOverrides = new DSDialogueOverride[0];

    public bool IsDSMode => dialogueContainer != null;

    public void StartDialogue()
    {
        BreakingCrystals crystal = GetComponent<BreakingCrystals>();
        if (crystal != null)
        {
            if (crystal.TryInterceptInteraction())
            {
                return;
            }
        }

        if (!gameObject.activeInHierarchy)
            return;

        DSDialogueSO startingDialogue = GetStartingDialogue();

        if (startingDialogue == null)
            return;

        FindFirstObjectByType<DialogueManager>().OpenDSDialogue(startingDialogue, actor, lockPlayerMovement, lockPlayerCamera);
    }

    private DSDialogueSO GetStartingDialogue()
    {
        foreach (DSDialogueOverride dialogueOverride in dialogueOverrides)
        {
            if (dialogueOverride.Dialogue == null)
                continue;

            if (DSDialogueFlags.Get(dialogueOverride.RequiredFlag) == dialogueOverride.RequiredFlagValue)
                return dialogueOverride.Dialogue;
        }

        if (!string.IsNullOrEmpty(startingDialogueName))
        {
            foreach (DSDialogueSO ungroupedDialogue in dialogueContainer.UngroupedDialogue)
            {
                if (ungroupedDialogue.DialogueName == startingDialogueName)
                    return ungroupedDialogue;
            }

            foreach (var dialogueGroup in dialogueContainer.DialogueGroups)
            {
                foreach (DSDialogueSO dialogue in dialogueGroup.Value)
                {
                    if (dialogue.DialogueName == startingDialogueName)
                        return dialogue;
                }
            }
        }

        foreach (DSDialogueSO ungroupedDialogue in dialogueContainer.UngroupedDialogue)
        {
            if (ungroupedDialogue.IsStartingDialogue)
                return ungroupedDialogue;
        }

        foreach (var dialogueGroup in dialogueContainer.DialogueGroups)
        {
            foreach (DSDialogueSO dialogue in dialogueGroup.Value)
            {
                if (dialogue.IsStartingDialogue)
                    return dialogue;
            }
        }

        if (dialogueContainer.UngroupedDialogue.Count > 0)
            return dialogueContainer.UngroupedDialogue[0];

        foreach (var dialogueGroup in dialogueContainer.DialogueGroups)
        {
            if (dialogueGroup.Value.Count > 0)
                return dialogueGroup.Value[0];
        }

        return null;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && DialogueManager.isActive)
        {
            FindFirstObjectByType<DialogueManager>().CloseDialogue();
        }
    }
}


[System.Serializable]
public class DSDialogueOverride
{
    [Tooltip("Nom du flag à vérifier (ex: Has_Apple)")]
    public string RequiredFlag;

    [Tooltip("Valeur attendue du flag pour que cet override s'active")]
    public bool RequiredFlagValue = true;

    [Tooltip("Dialogue de remplacement si la condition est vraie")]
    public DSDialogueSO Dialogue;
}

[System.Serializable]
public class Actor
{
    public string name;
}