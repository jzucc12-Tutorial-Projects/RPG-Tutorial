using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;


namespace GameDevTV.Assets.Dialogues.Editor
{
    public class DialogueEditor : EditorWindow 
    {
        Dialogue selectedDialogue = null;
        [NonSerialized] GUIStyle nodeStyle;
        [NonSerialized] GUIStyle playerNodeStyle;
        [NonSerialized] DialogueNode draggingNode = null;
        [NonSerialized] Vector2 draggingOffset;
        [NonSerialized] DialogueNode creatingNode = null;
        [NonSerialized] DialogueNode deletingNode = null;
        [NonSerialized] DialogueNode linkingParent = null;
        Vector2 scrollPosition;
        [NonSerialized] bool draggingCanvas = false;
        [NonSerialized] Vector2 draggingCanvasOffset;
        const float canvasSize = 4000;
        const float backgroundSize = 50;

        private void OnEnable() 
        {
            Selection.selectionChanged += OnSelectionChanged;

            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12 ,12, 12);

            playerNodeStyle = new GUIStyle();
            playerNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            playerNodeStyle.padding = new RectOffset(20, 20, 20, 20);
            playerNodeStyle.border = new RectOffset(12, 12 ,12, 12);
        }

        private void OnDisable() 
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        #region//Selection
        private void OnSelectionChanged()
        {
            Dialogue currentDialogue = Selection.activeObject as Dialogue;
            if(!currentDialogue) return;

            selectedDialogue = currentDialogue;
            Repaint();
        }

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if(dialogue == null) return false;
            
            ShowEditorWindow();
            return true;
        }
        #endregion

        #region //GUI
        private void OnGUI() 
        {
            if(selectedDialogue == null) 
            {
                EditorGUILayout.LabelField("No dialogue selected");
                return;
            }

            ProcessEvents();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            Rect canvas = GUILayoutUtility.GetRect(canvasSize, canvasSize);
            Texture2D background = Resources.Load<Texture2D>("background");
            Rect texCoords = new Rect(0, 0, canvasSize / backgroundSize, canvasSize / backgroundSize);
            GUI.DrawTextureWithTexCoords(canvas, background, texCoords);

            foreach(DialogueNode node in selectedDialogue.GetAllNodes())
                DrawConnections(node);

            foreach(DialogueNode node in selectedDialogue.GetAllNodes())
                DrawNode(node);

            EditorGUILayout.EndScrollView();

            if(creatingNode != null)
            {
                selectedDialogue.CreateNode(creatingNode);
                creatingNode = null;
            }

            if(deletingNode != null)
            {
                selectedDialogue.DeleteNode(deletingNode);
                deletingNode = null;
            }
        }

        void ProcessEvents()
        {
            if(Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                if(draggingNode != null)
                {
                    draggingOffset = draggingNode.GetPosition() - Event.current.mousePosition;
                    Selection.activeObject = draggingNode;
                }
                else
                {
                    draggingCanvas = true;
                    draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
                    Selection.activeObject = selectedDialogue;
                }

            }
            else if(Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                draggingNode.SetPosition(Event.current.mousePosition + draggingOffset);
                GUI.changed = true;
            }
            else if(Event.current.type == EventType.MouseDrag && draggingCanvas)
            {
                scrollPosition = draggingCanvasOffset - Event.current.mousePosition;
                GUI.changed = true;
            }
            else if(Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                draggingNode = null;
            }
            else if(Event.current.type == EventType.MouseUp && draggingCanvas)
                draggingCanvas = false;
        }

        private void DrawNode(DialogueNode node)
        {
            GUIStyle style = node.IsPlayerSpeaking() ? playerNodeStyle : nodeStyle;
            GUILayout.BeginArea(node.GetRect(), style);
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Node: ", EditorStyles.whiteLabel);
            string newText = EditorGUILayout.TextField(node.GetText());

            if (EditorGUI.EndChangeCheck())
            {
                node.SetText(newText);
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
                creatingNode = node;

            DrawLinkButtons(node);

            if (GUILayout.Button("-"))
                deletingNode = node;
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DrawLinkButtons(DialogueNode node)
        {
            if (linkingParent == null)
            {
                if (GUILayout.Button("link"))
                    linkingParent = node;
            }
            else if (node == linkingParent)
            {
                if (GUILayout.Button("cancel"))
                    linkingParent = null;
            }
            else if (linkingParent.GetChildren().Contains(node.name))
            {
                if (GUILayout.Button("unlink"))
                {
                    linkingParent.RemoveLink(node.name);
                    linkingParent = null;
                }
            }
            else
            {
                if (GUILayout.Button("child"))
                {
                    linkingParent.AddLink(node.name);
                    linkingParent = null;
                }
            }
        }

        void DrawConnections(DialogueNode node)
        {
            Vector3 startPosition = new Vector2(node.GetRect().xMax, node.GetRect().center.y);

            foreach(DialogueNode child in selectedDialogue.GetAllChildren(node))
            {
                Vector3 endPosition = new Vector2(child.GetRect().xMin, child.GetRect().center.y);
                Vector3 controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.7f;

                Handles.DrawBezier(startPosition, endPosition, 
                                    startPosition + controlPointOffset, 
                                    endPosition - controlPointOffset, 
                                    Color.white, null, 4f);
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 mousePosition)
        {
            DialogueNode selectedNode = null;
            foreach(DialogueNode node in selectedDialogue.GetAllNodes())
            {
                if(!node.GetRect().Contains(mousePosition)) continue;
                selectedNode = node;
            }
            return selectedNode;
        }
        #endregion
    }
}