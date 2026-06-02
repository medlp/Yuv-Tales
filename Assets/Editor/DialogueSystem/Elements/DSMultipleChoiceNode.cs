using UnityEngine;
using UnityEngine.UI;


namespace DS.Elements
{
    using Enumerations;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine.UIElements;
    using Utilities;

    public class DSMultipleChoiceNode : DSNode
    {
        public override void Initialize(Vector2 position)
        {
            base.Initialize(position);

            DialogueType = DSDialogueType.MultipleChoice;

            Choices.Add("New Choice");
        }

        public override void Draw()
        {
            base.Draw();

            /* MAIN CONTAINER */

            Button addChoiceButton = DSElementUtility.CreateButton("Add Choice", () =>
            {
                Port choicePort = CreateChoicePort("New choice");

                outputContainer.Add(choicePort);

                Choices.Add("New Choices");
            });

            addChoiceButton.AddToClassList("ds-node_button");

            mainContainer.Insert(1, addChoiceButton);

            /* OUTPUT CONTAINER */

            foreach (string choice in Choices)
            {
                Port choicePort = CreateChoicePort(choice);

                outputContainer.Add(choicePort);
            }

            RefreshExpandedState();

        }

        #region Elements Creation
        private Port CreateChoicePort(string choice)
        {
            Port choicePort = this.CreatePort();

            choicePort.portName = "";

            Button deleteChoiceButton = DSElementUtility.CreateButton("X");

            deleteChoiceButton.AddToClassList("ds-node_button");

            TextField choiceTextField = DSElementUtility.CreateTextField(choice);

            choiceTextField.AddClasses(
                "ds-node_textfield",
                "ds-node_choice-textfield",
                "ds-node_textfield_hidden"
                );

            choicePort.Add(choiceTextField);
            choicePort.Add(deleteChoiceButton);
            return choicePort;
        }
        #endregion
    }
}


