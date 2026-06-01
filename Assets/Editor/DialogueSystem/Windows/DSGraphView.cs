using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;

namespace DS.Windows
{
    using Elements;
    using Enumerations;
    using System;
    using System.Collections.Generic;

    public class DSGraphView : GraphView
    {
        public DSGraphView()
        {
            AddManipulators();

            AddGridBackground();

            AddStyles();
        }

        #region Overrided Methods
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePort = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort == port)
                {
                    return;
                }
                 
                if(startPort.node == port.node)
                {
                    return;
                }

                if(startPort.direction == port.direction)
                {
                    return;
                }

                compatiblePort.Add(port);
            });

            return compatiblePort;
        }
        #endregion

        #region Manipulators
        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(CreateNodeContextualMenu("Add Node (Single Choice)", DSDialogueType.SingleChoice));
            this.AddManipulator(CreateNodeContextualMenu("Add Node (Multiple Choice)", DSDialogueType.MultipleChoice));
            this.AddManipulator(CreateGroupContextualMenu());
        }

        private IManipulator CreateGroupContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
            menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => AddElement(CreateGroup("DialogGroup", contentViewContainer.WorldToLocal(actionEvent.eventInfo.localMousePosition))))
                );

            return contextualMenuManipulator;
        }

        private IManipulator CreateNodeContextualMenu(string actionTitle, DSDialogueType dialogueType)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent=> AddElement(CreateNode(dialogueType, contentViewContainer.WorldToLocal(actionEvent.eventInfo.localMousePosition))))
                );

            return contextualMenuManipulator;
        }
        #endregion

        #region Elements Creation
        private Group CreateGroup(string title, Vector2 localMousePosition)
        {
            Group group = new Group() 
            { 
                title = title
            };

            group.SetPosition(new Rect(localMousePosition, Vector2.zero));

            return group;
        }

        private DSNode CreateNode(DSDialogueType dialogueType, Vector2 position)
        {
            Type nodetype = Type.GetType($"DS.Elements.DS{dialogueType}Node"); 
            DSNode node = (DSNode) Activator.CreateInstance(nodetype);

            node.Initialize(position);
            node.Draw();

            AddElement(node);

            return node;
        }
        #endregion

        #region Elements Addition
        private void AddStyles()
        {
            StyleSheet graphStyleSheet = (StyleSheet) EditorGUIUtility.Load("Assets/Editor Default Ressources/DialogueSystem/DSGraphViewStyles.uss");
            StyleSheet nodeStyleSheet = (StyleSheet)EditorGUIUtility.Load("Assets/Editor Default Ressources/DialogueSystem/DSNodeStyles.uss");

            styleSheets.Add(graphStyleSheet);
            styleSheets.Add(nodeStyleSheet);
        }

        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();

            gridBackground.StretchToParentSize();

            Insert(0, gridBackground);
        }
        #endregion
    }
}


