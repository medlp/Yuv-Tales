using UnityEngine;

namespace DS.Elements
{
    using Enumerations;
    using Utilities;
    using UnityEditor.Experimental.GraphView;

    public class DSSingleChoiceNode : DSNode
    {
        public override void Initialize(Vector2 position)
        {
            base.Initialize(position);

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

