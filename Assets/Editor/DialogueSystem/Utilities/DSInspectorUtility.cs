using UnityEditor;
using UnityEngine;
using System;

namespace DS.Utilities
{
    public static class DSInspectorUtility
    {

        public static void DrawDisabledFields(Action action)
        {
            EditorGUI.BeginDisabledGroup(true);

            action.Invoke();

            EditorGUI.EndDisabledGroup();
        }
        public static void DrawHeader(string label)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        }

        public static void DrawHelpBox(string message, MessageType messageType = MessageType.Info, bool wide = true)
        {
            EditorGUILayout.HelpBox(message, messageType, wide);
        }

        public static void DrawPropertyField(this SerializedProperty serialedProperty)
        {
            EditorGUILayout.PropertyField(serialedProperty);
        }

        public static int DrawPopup(string label, SerializedProperty selectedIndexProperty, string[] options)
        {
            return EditorGUILayout.Popup(label, selectedIndexProperty.intValue, options);
        }   
        
        public static int DrawPopup(string label, int selectedIndex, string[] options)
        {
            return EditorGUILayout.Popup(label, selectedIndex, options);
        }

        public static void DrawSpace(int amount = 10)
        {
            EditorGUILayout.Space(amount);
        }

        public static void DrawPropertyWithSpacing(SerializedProperty property)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(property.displayName, GUILayout.Width(140));

            property.boolValue = EditorGUILayout.Toggle(property.boolValue, GUILayout.Width(20));

            EditorGUILayout.EndHorizontal();
        }
    }
}

