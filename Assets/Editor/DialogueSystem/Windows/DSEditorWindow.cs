using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


namespace DS.Windows
{
    using System;
    using Utilities;

    public class DSEditorWindow : EditorWindow
    {
        private DSGraphView graphView;
        private string defaultFileName = "DialogueFileName";

        private static TextField fileNameTextField;
        private Button saveButton;

        [MenuItem("Window/DS/Dialogue Graph")]
        public static void ShowExample()
        {
            DSEditorWindow wnd = GetWindow<DSEditorWindow>();
            wnd.titleContent = new GUIContent("Dialogue Graph");
        }

        private void OnEnable()
        {
            AddGraphView();
            AddToolBar();
            AddStyles();
        }


        #region Elements Addition


        private void AddGraphView()
        {
            graphView = new DSGraphView(this);

            graphView.StretchToParentSize();

            rootVisualElement.Add(graphView);
        }

        private void AddToolBar()
        {
            Toolbar toolbar = new Toolbar();

            toolbar.name = "ds-toolbar";

            fileNameTextField = DSElementUtility.CreateTextField(defaultFileName, "File Name :");

            saveButton = DSElementUtility.CreateButton("Save", () => Save());


            Button clearButton = DSElementUtility.CreateButton("Clear", () => Clear());
            Button resetButton = DSElementUtility.CreateButton("Reset", () => ResetGraph());

            toolbar.Add(fileNameTextField);
            toolbar.Add(saveButton);
            toolbar.Add(clearButton);
            toolbar.Add(resetButton);

            toolbar.AddStyleSheets("Assets/Editor Default Ressources/DialogueSystem/DSToolBarStyles.uss");

            rootVisualElement.Add(toolbar);
        }

        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("Assets/Editor Default Ressources/DialogueSystem/DSVariables.uss");
        }
        #endregion

        #region ToolBar Action
        private void Save()
        {
            if(string.IsNullOrEmpty(fileNameTextField.value))
            {
                EditorUtility.DisplayDialog(
                    "Invalid file name.",
                    "Please ensure the file name you have typed in is valid",
                    "Ok"
                );

                return;

            }

            DSIOUtility.Initialize(graphView, fileNameTextField.value);
            DSIOUtility.Save();
        }

        private void Clear()
        {
            graphView.ClearGraph();
        }

        private void ResetGraph()
        {
            Clear();

            UpdateFileName(defaultFileName);
        }

        #endregion

        #region Utility Methods
        public static void UpdateFileName(string newFileName)
        {
            fileNameTextField.value = newFileName;
        }
        public void EnableSaving()
        {
            saveButton.SetEnabled(true);
        }

        public void DisableSaving()
        {
            saveButton.SetEnabled(false);
        }
        #endregion
    }
}



