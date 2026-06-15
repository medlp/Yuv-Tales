using UnityEngine;
using System;

namespace DS.Data
{
    using ScriptableObjects;
    using System;

    [Serializable]
    public class DSDialogueChoiceData
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public DSDialogueSO NextDialogue { get; set; }

    }
}

