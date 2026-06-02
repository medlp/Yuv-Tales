using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace DS.Windows
{
    using Elements;
    using Enumerations;
    using Utilities;
    using Data.Error;
    using UnityEngine.Rendering;
    using UnityEngine.Analytics;

    public class DSGraphView : GraphView
    {

        private DSSearchWindow searchWindow;
        private DSEditorWindow editorWindow;

        private SerializableDictionary<string, DSNodeErrorData> ungroupedNodes;
        private SerializableDictionary<Group, SerializableDictionary<string, DSNodeErrorData>> groupedNodes;

        public DSGraphView(DSEditorWindow dsEditorWindow)
        {    
            editorWindow = dsEditorWindow;
            
            ungroupedNodes = new SerializableDictionary<string, DSNodeErrorData>();
            groupedNodes = new SerializableDictionary<Group, SerializableDictionary<string, DSNodeErrorData>>();

            AddManipulators();
            AddSearchWindow();
            AddGridBackground();

            OnElementsDeleted();
            OnGroupElementsAdded();

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

            node.Initialize(this, position);
            node.Draw();

            AddUngroupedNodes(node);
            //AddElement(node);

            return node;
        }
        #endregion

        #region CallBacks

        private void OnElementsDeleted()
        {
            deleteSelection = (operationName, askUser) =>
            {
                List<DSNode> nodesToDelete = new List<DSNode>();
                foreach (GraphElement element in selection)
                {
                    if(element is DSNode node)
                    {
                        nodesToDelete.Add(node);

                        continue;
                    }
                }

                foreach (DSNode node in nodesToDelete)
                {
                    RemoveUngroupedNodes(node);

                    RemoveElement(node);
                }
            };
        }

        private void OnGroupElementsAdded()
        {
            elementsAddedToGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is DSNode))
                    {
                        continue;
                    }

                    DSNode node = (DSNode)element;

                    RemoveUngroupedNodes(node);
                    AddGroupedNode(node, group);
                }
            };
        }

        private void OnGroupElementRemoved()
        {
            elementsRemovedFromGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is DSNode))
                    {
                        continue;
                    }

                    DSNode node = (DSNode)element;

                    RemoveGroupedNode(node, group);
                    AddUngroupedNodes(node);
                }
            };
        }

        private void RemoveGroupedNode(DSNode node, Group group)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Repeated Elements
        public void AddUngroupedNodes(DSNode node)
        {
            string nodeName = node.DialogueName;

            if(!ungroupedNodes.ContainsKey(nodeName))
            {
                DSNodeErrorData nodeErrorData = new DSNodeErrorData(); 
                
                nodeErrorData.Nodes.Add(node);

                ungroupedNodes.Add(nodeName, nodeErrorData);

                return;
            }

            List<DSNode> ungroupedNodesList = ungroupedNodes[nodeName].Nodes;

            ungroupedNodesList.Add(node);

            Color errorColor = ungroupedNodes[nodeName].ErrorData.color;

            node.SetErrorStyle(errorColor);

            if(ungroupedNodesList.Count == 2)
            {
                ungroupedNodesList[0].SetErrorStyle(errorColor);
            }
        }

        public void RemoveUngroupedNodes(DSNode node)
        {
            string nodeName = node.DialogueName;

            List<DSNode> ungroupedNodesList = ungroupedNodes[nodeName].Nodes;

            ungroupedNodes[nodeName].Nodes.Remove(node);

            node.ResetStyle();

            if (ungroupedNodes[nodeName].Nodes.Count == 1)
            {
                ungroupedNodes[nodeName].Nodes[0].ResetStyle();
            }

            if (ungroupedNodes[nodeName].Nodes.Count == 0)
            {
                ungroupedNodes.Remove(nodeName);
            }
        }

        private void AddGroupedNode(DSNode node, Group group)
        {
            string nodeName = node.DialogueName;

            if (!groupedNodes.ContainsKey(group))
            {
                groupedNodes.Add(group, new SerializableDictionary<string, DSNodeErrorData>());
            }

            if (!groupedNodes[group].ContainsKey(nodeName))
            {
                DSNodeErrorData nodeErrorData = new DSNodeErrorData();

                nodeErrorData.Nodes.Add(node);

                groupedNodes[group].Add(nodeName, nodeErrorData);

                return;
            }

            List<DSNode> groupedNodeList = groupedNodes[group][nodeName].Nodes;

            groupedNodeList.Add(node);
            Color errorColor = groupedNodes[group][nodeName].ErrorData.color;

            node.SetErrorStyle(errorColor);

            if (groupedNodeList.Count == 2)
            {
                groupedNodeList[0].SetErrorStyle(errorColor);
            }
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


