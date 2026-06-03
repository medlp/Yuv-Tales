using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


namespace DS.Windows
{
    using Utilities;

    public class DSEditorWindow : EditorWindow
    {
        private string defaultFileName = "DialogueFileName";
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
            DSGraphView graphView = new DSGraphView(this);

            graphView.StretchToParentSize();

            rootVisualElement.Add(graphView);
        }

        private void AddToolBar()
        {
            Toolbar toolbar = new Toolbar();

            toolbar.name = "ds-toolbar";

            TextField fileNameTextField = DSElementUtility.CreateTextField(defaultFileName, "File Name :");

            saveButton = DSElementUtility.CreateButton("Save");

            toolbar.Add(fileNameTextField);
            toolbar.Add(saveButton);

            toolbar.AddStyleSheets("Assets/Editor Default Ressources/DialogueSystem/DSToolBarStyles.uss");

            rootVisualElement.Add(toolbar);
        }
        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("Assets/Editor Default Ressources/DialogueSystem/DSVariables.uss");
        }
        #endregion

        #region Utility Methods
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



