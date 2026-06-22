using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;
using System.Linq;

namespace DS.Elements
{
    using Data.Save;
    using Windows;
    using Enumerations;
    using Utilities;
    using System;
    using System.Linq;

    public class DSNode : Node
    {
        public string ID { get; set; }
        public string DialogueName { get; set; }
        public List<DSChoiceSaveData> Choices { get; set; }
        public string Text { get; set; }
        public DSDialogueType DialogueType { get; set; }
        public DSGroup group { get; set; }
        public string ActorName { get; set; } = "";

        protected DSGraphView graphView;

        private Color defaultBackgroundColor;

        public virtual void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            ID = Guid.NewGuid().ToString();
            DialogueName = nodeName;
            Choices = new List<DSChoiceSaveData>();
            Text = "Dialogue text.";

            defaultBackgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);

            graphView = dsGraphView;
            SetPosition(new Rect(position, Vector2.zero));

            mainContainer.AddClasses("ds-node_main-container");
            extensionContainer.AddClasses("ds-node_extension-container");
        }

        public virtual void Draw()
        {
            /* TITLE CONTAINER */

            TextField dialogueNameTextField = DSElementUtility.CreateTextField(DialogueName, null, callback =>
            {
                TextField target = (TextField)callback.target;

                if (string.IsNullOrEmpty(target.value))
                {
                    if (!string.IsNullOrEmpty(DialogueName))
                        ++graphView.NameErrorsAmount;
                }
                else
                {
                    if (string.IsNullOrEmpty(DialogueName))
                        --graphView.NameErrorsAmount;
                }

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
                "ds-node_text-field",
                "ds-node_filename-text-field",
                "ds-node_text-field_hidden"
                );

            titleContainer.Insert(0, dialogueNameTextField);


            /* ACTOR CONTAINER */
            TextField actorNameTextField = DSElementUtility.CreateTextField(ActorName, "Name :", callback =>
            {
                ActorName = callback.newValue;
            });

            actorNameTextField.AddClasses(
                "ds-node_text-field",
                "ds-node_filename-text-field",
                "ds-node_text-field_hidden"
            );

            mainContainer.Insert(1, actorNameTextField);

            /* INPUT CONTAINER */

            Port inputPort = this.CreatePort("Dialogue Connection", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);

            inputPort.portName = "Dialogue Connection";

            inputContainer.Add(inputPort);

            /* EXTENSIONS CONTAINER */

            VisualElement customDataContainer = new VisualElement();

            customDataContainer.AddClasses("ds-node_custom-data-container");

            Foldout textFoldout = DSElementUtility.CreateFoldout("Dialogue Text");

            TextField textFoldoutTextField = DSElementUtility.CreateTextArea(Text, null, callback =>
            {
                Text = callback.newValue;
            });
            textFoldout.Add(textFoldoutTextField);

            textFoldoutTextField.AddClasses(
                "ds-node_text-field",
                "ds-node_quote-text-field"
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

        public bool IsStartingNode()
        {
            Port inputPort = (Port) inputContainer.Children().First();

            return !inputPort.connected;
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
