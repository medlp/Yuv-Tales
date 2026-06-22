using UnityEngine;
using System.Collections.Generic;
using DS.Utilities;


namespace DS.ScriptableObjects
{
    
    public class DSDialogueContainerSO : ScriptableObject
    {
        [field: SerializeField] public string FileName {  get; set; }

        [field: SerializeField] public SerializableDictionary<DSDialogueGroupSO, List<DSDialogueSO>> DialogueGroups { get; set; }
        [field: SerializeField] public List<DSDialogueSO> UngroupedDialogue {  get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;

            DialogueGroups = new SerializableDictionary<DSDialogueGroupSO, List<DSDialogueSO>>();
            UngroupedDialogue = new List<DSDialogueSO>();
        }

        public List<string> GetDialogueGroupNames()
        {
            List<string> dialogueGroupNames = new List<string>();

            foreach(DSDialogueGroupSO dialogueGroup in DialogueGroups.Keys)
            {
                dialogueGroupNames.Add(dialogueGroup.GroupName);   
            }

            return dialogueGroupNames;
        }

        public List<string> GetGroupedDialogueNames(DSDialogueGroupSO dialogueGroup, bool startingDialogueOnly)
        {
            List<DSDialogueSO> groupedDialogues = DialogueGroups[dialogueGroup];

            List<string> groupedDialogueNames = new List<string>();

            foreach (DSDialogueSO groupedDialogue in groupedDialogues)
            {
                if (startingDialogueOnly && !groupedDialogue.IsStartingDialogue)
                {
                    continue;
                }

                groupedDialogueNames.Add(groupedDialogue.DialogueName);
            }

            return groupedDialogueNames;

        }

        public List<string> GetUngroupedDialogueNames(bool startingDialogueOnly)
        {
            List<string> ungroupedDialogueNames = new List<string>();


            foreach (DSDialogueSO ungroupedDialogue in UngroupedDialogue)
            {
                if (startingDialogueOnly && !ungroupedDialogue.IsStartingDialogue)
                {
                    continue;
                }


                ungroupedDialogueNames.Add(ungroupedDialogue.DialogueName);
            }

            return ungroupedDialogueNames;

        }
    }
}

