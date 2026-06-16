using System;
using System.Collections.Generic;

namespace YuvTales.UI.Dialogue
{
    public class DialogueChoiceData
    {
        public string Text { get; set; }
        public Action OnSelected { get; set; }
    }

    public class DialogueData
    {
        public string ActorName { get; set; }
        public string MessageText { get; set; }
        public List<DialogueChoiceData> Choices { get; set; }

        public DialogueData()
        {
            Choices = new List<DialogueChoiceData>();
        }
    }
}
