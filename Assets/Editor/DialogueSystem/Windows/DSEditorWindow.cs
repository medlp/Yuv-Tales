using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace DS.Windows
{
    using Utilities;

    public class DSEditorWindow : EditorWindow
    {
        [MenuItem("Window/DS/Dialogue Graph")]
        public static void ShowExample()
        {
            DSEditorWindow wnd = GetWindow<DSEditorWindow>();
            wnd.titleContent = new GUIContent("Dialogue Graph");
        }

        private void OnEnable()
        {

            AddGraphView();

            AddStyles();
        }

        #region Elements Addition
        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("Assets/Editor Default Ressources/DialogueSystem/DSVariables.uss");
        }

        private void AddGraphView()
        {
            DSGraphView graphView = new DSGraphView(this);

            graphView.StretchToParentSize();

            rootVisualElement.Add(graphView);
        }
        #endregion
    }
}



