using UnityEngine;

namespace DS.Elements
{
    using Windows;
    using Enumerations;
    using UnityEditor.Experimental.GraphView;
    using Utilities;

    public class DSSingleChoiceNode : DSNode
    {
        public override void Initialize(DSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(dsGraphView, position);

            DialogueType = DSDialogueType.SingleChoice;

            Choices.Add("Next Dialogue");
        }

        public override void Draw()
        {
            base.Draw();

            /* OUTPUT CONTAINER */

            foreach (string choice in Choices) 
            {
                Port choicePort = this.CreatePort(choice);

                choicePort.portName = choice;

                outputContainer.Add(choicePort);
            }

            RefreshExpandedState();

        }
    }
}

