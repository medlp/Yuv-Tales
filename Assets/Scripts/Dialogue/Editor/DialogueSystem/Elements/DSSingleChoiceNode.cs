using UnityEngine;

namespace DS.Elements
{
    using Data.Save;
    using Windows;
    using Enumerations;
    using UnityEditor.Experimental.GraphView;
    using Utilities;
    using UnityEngine.UIElements;

    public class DSSingleChoiceNode : DSNode
    {
        public override void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);

            DialogueType = DSDialogueType.SingleChoice;

            DSChoiceSaveData choiceData = new DSChoiceSaveData()
            {
                Text = "Next Dialogue"
            };

            Choices.Add(choiceData);
        }

        public override void Draw()
        {
            base.Draw();

            /* OUTPUT CONTAINER */

            foreach (DSChoiceSaveData choice in Choices) 
            {
                Port choicePort = this.CreatePort(choice.Text);
                choicePort.userData = choice;
                outputContainer.Add(choicePort);

                VisualElement conditionBlock = CreateConditionBlock(choice, choicePort);
                extensionContainer.Add(conditionBlock);
            }

            RefreshExpandedState();

        }
    }
}

