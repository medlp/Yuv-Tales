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
    using Utilities;

    public class DSGraphView : GraphView
    {

        private DSSearchWindow searchWindow;
        private DSEditorWindow editorWindow;

        public DSGraphView(DSEditorWindow dsEditorWindow)
        {
            AddManipulators();
            AddSearchWindow();
            AddGridBackground();
            AddStyles();
            editorWindow = dsEditorWindow;
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
            menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => AddElement(CreateGroup("DialogGroup", GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
                );

            return contextualMenuManipulator;
        }

        private IManipulator CreateNodeContextualMenu(string actionTitle, DSDialogueType dialogueType)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(dialogueType, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
                );

            return contextualMenuManipulator;
        }
        #endregion

        #region Elements Creation
        public Group CreateGroup(string title, Vector2 localMousePosition)
        {
            Group group = new Group() 
            { 
                title = title
            };

            group.SetPosition(new Rect(localMousePosition, Vector2.zero));

            AddElement(group);

            return group;
        }

        public DSNode CreateNode(DSDialogueType dialogueType, Vector2 position)
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
            this.AddStyleSheets(
                "Assets/Editor Default Ressources/DialogueSystem/DSGraphViewStyles.uss",
                "Assets/Editor Default Ressources/DialogueSystem/DSNodeStyles.uss"
                );


        }

        private void AddSearchWindow()
        {
            if (searchWindow == null)
            {
                searchWindow = ScriptableObject.CreateInstance<DSSearchWindow>();

                searchWindow.Initialize(this);
            }

            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
        }


        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();

            gridBackground.StretchToParentSize();

            Insert(0, gridBackground);
        }
        #endregion

        #region Utilities
        public Vector2 GetLocalMousePosition(Vector2 position, bool isSearchWindow = false)
        {
            Vector2 worldMousePosition = position;

            if (isSearchWindow)
            {
                worldMousePosition = worldMousePosition - editorWindow.position.position;
            }

            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);

            return localMousePosition;
        }
        #endregion

    }
}


