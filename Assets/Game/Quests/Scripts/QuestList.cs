using System;
using System.Collections.Generic;
using GameDevTV.Inventories;
using GameDevTV.Saving;
using UnityEngine;
using GameDevTV.Assets.Core;

namespace RPG.Quests
{
    public class QuestList : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        [SerializeField] List<QuestStatus> statuses = new List<QuestStatus>();
        public event Action ListUpdated;

        public IEnumerable<QuestStatus> GetStatuses() { return statuses; }

        public void AddQuest(Quest quest)
        {
            if(HasQuest(quest)) return;
            QuestStatus newStatus = new QuestStatus(quest);
            statuses.Add(newStatus);
            ListUpdated?.Invoke();
        }

        public void CompleteObjective(Quest quest, string objective)
        {
            QuestStatus status = GetQuestStatus(quest);
            status.CompleteObjective(objective);
            if(status.IsComplete())
                GiveReward(quest);

            ListUpdated?.Invoke();
        }

        public bool HasQuest(Quest quest)
        {
            return GetQuestStatus(quest) != null;
        }

        QuestStatus GetQuestStatus(Quest quest)
        {
            foreach(QuestStatus status in GetStatuses())
            {
                if(status.GetQuest() == quest)
                    return status;
            }

            return null;
        }

        void GiveReward(Quest quest)
        {
            foreach(Reward reward in quest.GetRewards())
            {
                bool success = GetComponent<Inventory>().AddToFirstEmptySlot(reward.item, reward.number);
                if(!success)
                {
                    GetComponent<ItemDropper>().DropItem(reward.item, reward.number);
                }
            }
        }

        public object CaptureState()
        {
            List<object> state = new List<object>();
            foreach(QuestStatus status in statuses)
            {
                state.Add(status.CaptureState());
            }
            return state;
        }

        public void RestoreState(object state)
        {
            List<object> restoreState = state as List<object>;
            if(restoreState == null) return;

            statuses.Clear();
            foreach(object objectState in restoreState)
            {
                statuses.Add(new QuestStatus(objectState));
            }
        }

        public bool? Evaluate(string predicate, string[] parameters)
        {
            switch(predicate)
            {
                case "HasQuest":
                return HasQuest(Quest.GetByName(parameters[0]));
                
                case "CompletedQuest":
                return GetQuestStatus(Quest.GetByName(parameters[0])).IsComplete();

                case "CompletedObjective":
                return GetQuestStatus(Quest.GetByName(parameters[0])).IsObjectiveComplete(parameters[1]);
            }

            return null;
        }
    }

}