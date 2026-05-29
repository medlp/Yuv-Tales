using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DSEditorWindow : EditorWindow
{
    [MenuItem("Window/UI Toolkit/DSEditorWindow")]
    public static void ShowExample()
    {
        DSEditorWindow wnd = GetWindow<DSEditorWindow>();
        wnd.titleContent = new GUIContent("DSEditorWindow");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);

    }
}
