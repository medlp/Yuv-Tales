using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;

namespace DS.Elements
{
    using Windows;
    using Enumerations;
    using Utilities;

    public class DSNode : Node
    {
        public string DialogueName { get; set; }
        public List<string> Choices { get; set; }
        public string Text { get; set; }

        private Color defaultBackgroundColor;
        private DSGraphView graphView;

        public DSDialogueType DialogueType { get; set; }
        public Group group { get; set; }

        public virtual void Initialize(DSGraphView dsGraphView, Vector2 position)
        {
            DialogueName = "DialogueName";
            Choices = new List<string>();
            Text = "Dialogue text.";

            defaultBackgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);

            graphView = dsGraphView;
            SetPosition(new Rect(position, Vector2.zero));

            mainContainer.AddClasses(".ds-node_main-container");
            extensionContainer.AddClasses(".ds-node_extension-container");
        }

        public virtual void Draw()
        {
            /* TITLE CONTAINER */

            TextField dialogueNameTextField = DSElementUtility.CreateTextField(DialogueName, callback =>
            {
                if (group == null)
                {
                    graphView.RemoveUngroupedNodes(this);

                    DialogueName = callback.newValue;

                    graphView.AddUngroupedNodes(this);

                    return;
                }

                DSGroup currentGroup = (DSGroup)group;

                graphView.RemoveGroupedNode(this, group);

                DialogueName = callback.newValue;

                graphView.AddGroupedNode(this, currentGroup);

            });

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

        #region Overrided Methods
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Disconnect Input Ports", actionEvent => DisconnectInputPorts());
            evt.menu.AppendAction("Disconnect Output Ports", actionEvent => DisconnectOutputPorts());

            base.BuildContextualMenu(evt);
        }
        #endregion


        #region Utility Methods
        public void DisconnectAllPorts()
        {
            DisconnectInputPorts();
            DisconnectOutputPorts();
        }


        private void DisconnectInputPorts()
        {
            DisconnectPorts(inputContainer);
        }

        private void DisconnectOutputPorts()
        {
            DisconnectPorts(outputContainer);
        }

        private void DisconnectPorts(VisualElement container)
        {
            foreach(Port port in container.Children())
            {
                if (!port.connected)
                {
                    continue;
                }

                graphView.DeleteElements(port.connections);
            }
        }

        public void SetErrorStyle(Color color)
        {
            mainContainer.style.backgroundColor = color;
        }

        public void ResetStyle()
        {
            mainContainer.style.backgroundColor = defaultBackgroundColor;
        }
        #endregion

    }
}
