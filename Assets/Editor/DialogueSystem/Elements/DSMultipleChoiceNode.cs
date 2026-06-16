using UnityEngine;

namespace DS.Elements
{
    using Data.Save;
    using Windows;
    using Enumerations;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine.UIElements;
    using Utilities;

    public class DSMultipleChoiceNode : DSNode
    {
        public override void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);

            DialogueType = DSDialogueType.MultipleChoice;

            DSChoiceSaveData choiceData = new DSChoiceSaveData()
            {
                Text = "New Choice"
            };

            Choices.Add(choiceData);
        }

        public override void Draw()
        {
            base.Draw();

            /* MAIN CONTAINER */
            Button addChoiceButton = DSElementUtility.CreateButton("Add Choice", () =>
            {
                DSChoiceSaveData choiceData = new DSChoiceSaveData()
                {
                    Text = "New Choice"
                };

                Choices.Add(choiceData);

                Port choicePort = CreateChoicePort(choiceData);
                outputContainer.Add(choicePort);

                VisualElement conditionBlock = CreateConditionBlock(choiceData, choicePort); 
                extensionContainer.Add(conditionBlock);
            });

            addChoiceButton.AddToClassList("ds-node_button");
            mainContainer.Insert(1, addChoiceButton);

            foreach (DSChoiceSaveData choice in Choices)
            {
                Port choicePort = CreateChoicePort(choice);
                outputContainer.Add(choicePort);

                VisualElement conditionBlock = CreateConditionBlock(choice, choicePort);
                extensionContainer.Add(conditionBlock);
            }

            RefreshExpandedState();
        }

        #region Elements Creation
        private Port CreateChoicePort(DSChoiceSaveData choiceData)
        {
            Port choicePort = this.CreatePort();
            choicePort.userData = choiceData;

            Button deleteChoiceButton = DSElementUtility.CreateButton("X", () =>
            {
                if (Choices.Count == 1) return;

                if (choicePort.connected)
                    graphView.DeleteElements(choicePort.connections);

                Choices.Remove(choiceData);
                graphView.RemoveElement(choicePort);

                // Supprimer le bloc de condition associé
                if (choiceData.ConditionBlock != null)
                    extensionContainer.Remove(choiceData.ConditionBlock);
            });

            deleteChoiceButton.AddToClassList("ds-node_button");

            TextField choiceTextField = DSElementUtility.CreateTextField(choiceData.Text, null, callback =>
            {
                choiceData.Text = callback.newValue;
            });

            choiceTextField.AddClasses(
                "ds-node_text-field",
                "ds-node_choice-text-field",
                "ds-node_text-field_hidden"
            );

            choicePort.Add(choiceTextField);
            choicePort.Add(deleteChoiceButton);

            return choicePort;
        }

        private VisualElement CreateConditionBlock(DSChoiceSaveData choiceData, Port choicePort)
        {
            Foldout foldout = DSElementUtility.CreateFoldout($"Condition : {choiceData.Text}", collapsed: true);
            foldout.AddToClassList("ds-node_conditions-foldout");

            choiceData.OnTextChanged += newText => foldout.text = $"Condition : {newText}";

            Label requiredLabel = new Label("Pre-requisites");
            requiredLabel.AddToClassList("ds-node_condition-label");

            TextField requiredFlagField = DSElementUtility.CreateTextField(choiceData.RequiredFlag, "Flag :", callback =>
            {
                choiceData.RequiredFlag = callback.newValue;
            });

            Toggle requiredFlagToggle = new Toggle("Required value :");
            requiredFlagToggle.value = choiceData.RequiredFlagValue;
            requiredFlagToggle.RegisterValueChangedCallback(callback =>
            {
                choiceData.RequiredFlagValue = callback.newValue;
            });

            Label onChosenLabel = new Label("Achievements");
            onChosenLabel.AddToClassList("ds-node_condition-label");

            TextField onChosenFlagField = DSElementUtility.CreateTextField(choiceData.OnChosenFlag, "Flag :", callback =>
            {
                choiceData.OnChosenFlag = callback.newValue;
            });

            Toggle onChosenFlagToggle = new Toggle("Set Value :");
            onChosenFlagToggle.value = choiceData.OnChosenFlagValue;
            onChosenFlagToggle.RegisterValueChangedCallback(callback =>
            {
                choiceData.OnChosenFlagValue = callback.newValue;
            });

            foldout.Add(requiredLabel);
            foldout.Add(requiredFlagField);
            foldout.Add(requiredFlagToggle);
            foldout.Add(onChosenLabel);
            foldout.Add(onChosenFlagField);
            foldout.Add(onChosenFlagToggle);

            choiceData.ConditionBlock = foldout;

            return foldout;
        }
        #endregion
    }
}
