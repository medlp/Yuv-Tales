using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;

namespace DS.Elements
{

    using Enumerations;
    using Utilities;

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

            mainContainer.AddClasses(".ds-node_main-container");
            extensionContainer.AddClasses(".ds-node_extension-container");
        }

        public virtual void Draw()
        {
            /* TITLE CONTAINER */

            TextField dialogueNameTextField = DSElementUtility.CreateTextField(DialogueName);

            dialogueNameTextField.AddClasses(
                "ds-node_textfield", 
                "ds-node_filename-textfield", 
                "ds-node_textfield_hidden"
                );

            titleContainer.Insert(0, dialogueNameTextField);

            /* INPUT CONTAINER */

            Port inputPort = this.CreatePort("Dialogue Connection", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);

            inputPort.portName = "Dialogue Connection";

            inputContainer.Add(inputPort);

            /* EXTENSIONS CONTAINER */

            VisualElement customDataContainer = new VisualElement();

            customDataContainer.AddClasses("ds-node_custom-data-container");

            Foldout textFoldout = DSElementUtility.CreateFoldout("Dialogue Text");

            TextField textFoldoutTextField = DSElementUtility.CreateTextArea(Text);
            textFoldout.Add(textFoldoutTextField);

            textFoldoutTextField.AddClasses(
                "ds-node_textfield", 
                "ds-node_quote-textfield"
                );

            customDataContainer.Add(textFoldout);

            extensionContainer.Add(customDataContainer);

        }

    }
}
