using UnityEngine;

using DS.ScriptableObjects;
using System;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Graph")]
    public DSDialogueContainerSO dialogueContainer;
    public string startingDialogueName = "";
    public Actor actor;

    public bool IsDSMode => dialogueContainer != null;

    public void StartDialogue()
    {

        DSDialogueSO startingDialogue = GetStartingDialogue();

        if (startingDialogue == null)
        {
            return;
        }

        FindFirstObjectByType<DialogueManager>().OpenDSDialogue(startingDialogue, actor);

    }

    private DSDialogueSO GetStartingDialogue()
    {
        if(string.IsNullOrEmpty(startingDialogueName))
        {
            foreach(DSDialogueSO ungroupedDialogue in dialogueContainer.UngroupedDialogue)
            {
                if (ungroupedDialogue.DialogueName == startingDialogueName)
                    return ungroupedDialogue;
            }

            foreach(var dialogueGroup in dialogueContainer.DialogueGroups)
            {
                foreach(DSDialogueSO dialogue in dialogueGroup.Value)
                {
                    if(dialogue.DialogueName == startingDialogueName)
                    {
                        return dialogue;
                    }
                }
            }
        }

        foreach(DSDialogueSO ungroupedDialogue in dialogueContainer.UngroupedDialogue)
        {
            if (ungroupedDialogue.IsStartingDialogue)
            {
                return ungroupedDialogue;
            }
        }

        foreach(var dialogueGroup in dialogueContainer.DialogueGroups)
        {
            foreach (DSDialogueSO dialogue in dialogueGroup.Value)
            {
                if (dialogue.IsStartingDialogue)
                {
                    return dialogue;
                }
            }
        }

        if (dialogueContainer.UngroupedDialogue.Count > 0)
        {
            return dialogueContainer.UngroupedDialogue[0];
        }

        foreach (var dialogueGroup in dialogueContainer.DialogueGroups)
        {
            if (dialogueGroup.Value.Count > 0)
            {
                return dialogueGroup.Value[0];
            }
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
public class Actor 
{ 
    public string name;
    public Sprite sprite;
}