using System;
using System.Collections.Generic;
using System.Linq;
using GameDevTV.Assets.Core;
using UnityEngine;

namespace GameDevTV.Assets.Dialogues
{
    public class PlayerConversant : MonoBehaviour
    {
        Dialogue currentDialogue;
        DialogueNode currentNode = null;
        bool isChoosing = false;
        public event Action OnConversationUpdated;
        AIConversant currentConversant = null;
        [SerializeField] string myName = "Neo";

        public void StartDialogue(AIConversant newConversant, Dialogue newDialogue)
        {
            currentConversant = newConversant;
            currentDialogue = newDialogue;
            DialogueNode[] roots = FilterOnCondition(currentDialogue.GetRootNodes()).ToArray();
            int response = UnityEngine.Random.Range(0, roots.Length);
            currentNode = roots[0];
            TriggerEnterAction();
            OnConversationUpdated?.Invoke();
        }

        public bool IsActive() { return currentDialogue != null; }

        public bool IsChoosing()
        {
            return isChoosing;
        }

        public string GetText() 
        { 
            if(currentNode == null) return "";
            return currentNode.GetText(); 
        }

        public string GetCurrentSpeaker()
        {
            if(currentNode == null) return "";
            if(currentNode.IsPlayerSpeaking() || isChoosing) return myName;
            return currentConversant.GetName();
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode));
        }

        public void SelectChoice(DialogueNode chosenNode)
        {
            currentNode = chosenNode;
            TriggerEnterAction();
            isChoosing = false;
            Next();
        }

        public void Next()
        {
            int playerResponseCount = FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode)).Count();
            if(currentDialogue.GetAllowChoices() && playerResponseCount > 0)
            {
                isChoosing = true;
                TriggerExitAction();
                OnConversationUpdated?.Invoke();
                return;
            }

            DialogueNode[] children;
            if(currentDialogue.GetAllowChoices()) children = FilterOnCondition(currentDialogue.GetAIChildren(currentNode)).ToArray();
            else if(currentNode.IsPlayerSpeaking()) children = FilterOnCondition(currentDialogue.GetAIChildren(currentNode)).ToArray();
            else children = currentDialogue.GetPlayerChildren(currentNode).ToArray();
            
            int response = UnityEngine.Random.Range(0, children.Length);
            TriggerExitAction();
            currentNode = children[response];
            TriggerEnterAction();
            OnConversationUpdated?.Invoke();
        }

        public void Quit()
        {
            TriggerExitAction();
            currentDialogue = null;
            currentNode = null;
            isChoosing = false;
            currentConversant = null;
            OnConversationUpdated?.Invoke();
        }

        public bool HasNext()
        {
            var children = FilterOnCondition(currentDialogue.GetAllChildren(currentNode)).Count();
            return children > 0;
        }

        IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> inputNode)
        {
            foreach(var node in inputNode)
            {
                if(!node.CheckCondition(GetEvaluators()))
                {
                    continue;
                } 
                yield return node;
            }
        }

        IEnumerable<IPredicateEvaluator> GetEvaluators()
        {
            return GetComponents<IPredicateEvaluator>();
        }

        private void TriggerEnterAction()
        {
            if (!currentNode) return;
            TriggerAction(currentNode.GetOnEnterAction());

        }

        private void TriggerExitAction()
        {
            if (!currentNode) return;
            TriggerAction(currentNode.GetOnExitAction());
        }

        private void TriggerAction(string action)
        {
            if(string.IsNullOrEmpty(action)) return;
            foreach(DialogueTrigger trigger in currentConversant.GetComponents<DialogueTrigger>())
                trigger.Trigger(action);
        }
    }
}
