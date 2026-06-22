using UnityEngine;
using System;
using UnityEngine.UIElements;

namespace DS.Data.Save
{
    [Serializable]
    public class DSChoiceSaveData
    {
        [field: SerializeField] public string NodeID { get; set; }

        [field: SerializeField] public string RequiredFlag { get; set; }
        [field: SerializeField] public bool RequiredFlagValue { get; set; }

        [field: SerializeField] public string OnChosenFlag { get; set; }
        [field: SerializeField] public bool OnChosenFlagValue { get; set; }

        [NonSerialized] public VisualElement ConditionBlock;

        [NonSerialized] public Action<string> OnTextChanged;

        [SerializeField] private string _text;
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
