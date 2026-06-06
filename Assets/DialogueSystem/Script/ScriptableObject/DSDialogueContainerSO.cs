using UnityEngine;
using System;
using System.Collections.Generic;
using DS.Utilities;


namespace DS.ScriptableObjects
{
    
    public class DSDialogueContainerSO : ScriptableObject
    {
        [field: SerializeField] public string FileName {  get; set; }

        public SerializableDictionary<DSDialogueGroupSO, List<DSDialogueSO>> DialogueGroups { get; set; }
        public List<DSDialogueSO> UngroupedDialogue {  get; set; }

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
    }
}

