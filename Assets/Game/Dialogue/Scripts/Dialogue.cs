using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace GameDevTV.Assets.Dialogues
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] bool allowChoices = true;
        [SerializeField] List<DialogueNode> nodes = new List<DialogueNode>();
        Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();
        Vector2 newNodeOffset = new Vector2(250, 0);

        #if UNITY_EDITOR
        private void Awake() 
        {
            OnValidate();
        }
        #endif

        private void OnValidate()
        {
            nodeLookup.Clear();
            foreach(DialogueNode node in GetAllNodes())
            {
                nodeLookup[node.name] = node;
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        public IEnumerable<DialogueNode> GetRootNodes() 
        { 
            foreach(DialogueNode node in nodes)
            {
                if(!node.GetIsRoot()) continue;
                yield return node;
            }
        }

        public bool GetAllowChoices() { return allowChoices; }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            foreach(string child in parentNode.GetChildren())
            {
                if(!nodeLookup.ContainsKey(child)) continue;
                yield return nodeLookup[child];
            }
        }

        public IEnumerable<DialogueNode> GetPlayerChildren(DialogueNode currentNode)
        {
            foreach(DialogueNode node in GetAllChildren(currentNode))
            {
                if(!node.IsPlayerSpeaking()) continue;
                yield return node;
            }
        }

        public IEnumerable<DialogueNode> GetAIChildren(DialogueNode currentNode)
        {
            foreach(DialogueNode node in GetAllChildren(currentNode))
            {
                if(node.IsPlayerSpeaking()) continue;
                yield return node;
            }
        }

#if UNITY_EDITOR
        public void CreateNode(DialogueNode parent)
        {
            DialogueNode newNode = MakeNode(parent);

            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            Undo.RecordObject(this, "Added Dialogue Node");
            AddNode(newNode);
        }

        private DialogueNode MakeNode(DialogueNode parent)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();

            if (parent != null)
            {
                parent.AddLink(newNode.name);
                newNode.SetPlayerSpeaking(!parent.IsPlayerSpeaking());
                newNode.SetPosition(parent.GetPosition() + newNodeOffset);
            }
            return newNode;
        }

        private void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode);
            OnValidate();
        }

        public void DeleteNode(DialogueNode toDelete)
        {
            Undo.RecordObject(this, "Deleted Dialogue Node");
            nodes.Remove(toDelete);
            OnValidate();
            RemoveDanglingChildren(toDelete);
            Undo.DestroyObjectImmediate(toDelete);
        }

        private void RemoveDanglingChildren(DialogueNode toDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.RemoveLink(toDelete.name);
            }
        }
#endif
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if(nodes.Count == 0)
            {
                DialogueNode newNode = MakeNode(null);
                AddNode(newNode);
            }

            if(AssetDatabase.GetAssetPath(this) != "")
            {
                foreach(DialogueNode node in GetAllNodes())
                {
                    if(AssetDatabase.GetAssetPath(node) == "")
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {
        }
    }
}