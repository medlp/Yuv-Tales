using UnityEngine;
using System;

namespace DS.Data
{
    using ScriptableObjects;

    [Serializable]
    public class DSDialogueChoiceData
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public DSDialogueSO NextDialogue { get; set; }

        // Condition d'accès : ce choix n'est visible que si ce flag vaut RequiredFlagValue
        [field: SerializeField] public string RequiredFlag { get; set; }
        [field: SerializeField] public bool RequiredFlagValue { get; set; }

        // Effet : choisir cette option set ce flag à OnChosenFlagValue
        [field: SerializeField] public string OnChosenFlag { get; set; }
        [field: SerializeField] public bool OnChosenFlagValue { get; set; }
    }
}
