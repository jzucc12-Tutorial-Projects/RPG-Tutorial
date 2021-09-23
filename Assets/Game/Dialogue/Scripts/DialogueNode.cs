using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GameDevTV.Assets.Core;

namespace GameDevTV.Assets.Dialogues
{
    [System.Serializable]
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] bool isPlayerSpeaking = false;
        [SerializeField] [TextArea(10,100)] string text;
        [SerializeField] List<string> children = new List<string>();
        [SerializeField] Rect rect = new Rect(10, 10, 200, 100);
        [SerializeField] bool isRoot = false;
        [SerializeField] string onEnterAction = "";
        [SerializeField] string onExitAction = "";
        [SerializeField] Condition condition = null;

        #region //Getters
        public string GetText() { return text; }
        public Vector2 GetPosition() { return rect.position; }
        public Rect GetRect() { return rect; }
        public List<string> GetChildren() { return children; }
        public bool IsPlayerSpeaking() { return isPlayerSpeaking; }
        public string GetOnEnterAction() { return onEnterAction; }
        public string GetOnExitAction() { return onExitAction; }
        public bool GetIsRoot() { return isRoot; }
        #endregion

        public bool CheckCondition(IEnumerable<IPredicateEvaluator> evaluators)
        {
            return condition.Check(evaluators);
        }

#if UNITY_EDITOR
        #region //Setters
        public void SetPlayerSpeaking(bool newSpeaking)
        {
            Undo.RecordObject(this, "Change Dialogue Speaker");
            isPlayerSpeaking = newSpeaking;
        }

        public void SetText(string newText)
        {
            Undo.RecordObject(this, "Update Dialogue Text");
            text = newText;
            EditorUtility.SetDirty(this);
        }

        public void SetPosition(Vector2 newPosition)
        {
            Undo.RecordObject(this, "Move dialogue node");
            rect.position = newPosition;
            EditorUtility.SetDirty(this);
        }

        public void AddLink(string newLink)
        {
            Undo.RecordObject(this, "Add Dialogue Link");
            children.Add(newLink);
            EditorUtility.SetDirty(this);
        }

        public void RemoveLink(string toRemoveLink)
        {
            Undo.RecordObject(this, "Remove Dialogue Link");
            children.Remove(toRemoveLink);
            EditorUtility.SetDirty(this);
        }
        #endregion
    }
#endif
}
