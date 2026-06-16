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

        [field: SerializeField] public string RequiredFlag { get; set; }
        [field: SerializeField] public bool RequiredFlagValue { get; set; }

        [field: SerializeField] public string OnChosenFlag { get; set; }
        [field: SerializeField] public bool OnChosenFlagValue { get; set; }
    }
}
