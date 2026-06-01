using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;

namespace DS.Elements
{

    using Enumerations;

    public class DSNode : Node
    {
        public string DialogueName { get; set; }
        public List<string> Choices { get; set; }
        public string Text { get; set; }

        public DSDialogueType DialogueType { get; set; }

        public virtual void Initialize(Vector2 position)
        {
            DialogueName = "DialogueName";
            Choices = new List<string>();
            Text = "Dialogue text.";

            SetPosition(new Rect(position, Vector2.zero));

            mainContainer.AddToClassList(".ds-node_main-container");
            extensionContainer.AddToClassList(".ds-node_extension-container");
        }

        public virtual void Draw()
        {
            /* TITLE CONTAINER */

            TextField dialogueNameTextField = new TextField()
            {
                value = DialogueName
            };

            dialogueNameTextField.AddToClassList("ds-node_textfield");
            dialogueNameTextField.AddToClassList("ds-node_filename-textfield");
            dialogueNameTextField.AddToClassList("ds-node_textfield_hidden");


            titleContainer.Insert(0, dialogueNameTextField);

            /* INPUT CONTAINER */

            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));

            inputPort.portName = "Dialogue Connection";

            inputContainer.Add(inputPort);

            /* EXTENSIONS CONTAINER */

            VisualElement customDataContainer = new VisualElement();

            customDataContainer.AddToClassList("ds-node_custom-data-container");

            Foldout textFoldout = new Foldout()
            {
                text = "Dialogue Text"
            };

            TextField textFoldoutTextField = new TextField()
            {
                value = Text
            };
            textFoldout.Add(textFoldoutTextField);

            textFoldoutTextField.AddToClassList("ds-node_textfield");
            textFoldoutTextField.AddToClassList("ds-node_quote-textfield");


            customDataContainer.Add(textFoldout);

            extensionContainer.Add(customDataContainer);

        }

    }
}
