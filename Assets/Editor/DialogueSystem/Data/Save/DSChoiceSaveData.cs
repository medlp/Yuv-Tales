using UnityEngine;
using System;
using UnityEngine.UIElements;

namespace DS.Data.Save
{
    [Serializable]
    public class DSChoiceSaveData
    {
        [field: SerializeField] public string NodeID { get; set; }

        // Condition d'accès
        [field: SerializeField] public string RequiredFlag { get; set; }
        [field: SerializeField] public bool RequiredFlagValue { get; set; }

        // Effet au choix
        [field: SerializeField] public string OnChosenFlag { get; set; }
        [field: SerializeField] public bool OnChosenFlagValue { get; set; }

        // Référence au bloc visuel dans extensionContainer (non sérialisé)
        [NonSerialized] public VisualElement ConditionBlock;

        // Event pour sync le label du foldout quand le texte change (non sérialisé)
        [NonSerialized] public Action<string> OnTextChanged;

        private string _text;
        [field: SerializeField]
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnTextChanged?.Invoke(_text);
            }
        }
    }
}
