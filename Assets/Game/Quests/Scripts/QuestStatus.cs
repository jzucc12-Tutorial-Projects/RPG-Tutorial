using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestStatus
    {
        Quest quest = null;
        List<string> completedObjectives = new List<string>();
        string nextObjective;

        public QuestStatus(Quest quest)
        {
            this.quest = quest;
            nextObjective = quest.GetNextObjective(0);
        }

        public QuestStatus(object state)
        {
            QuestStatusRecord record = state as QuestStatusRecord;
            if(record == null) return;

            quest = Quest.GetByName(record.questName);
            completedObjectives = record.completedObjectives;
            nextObjective = quest.GetNextObjective(record.completedObjectives.Count);
        }

        public Quest GetQuest() { return quest; }
        public int GetCompletedCount() { return completedObjectives.Count; }
        public bool IsObjectiveComplete(string objective) { return completedObjectives.Contains(objective); }
        public void CompleteObjective(string objective) 
        {
            if(!quest.HasObjective(objective)) return; 
            if(completedObjectives.Contains(objective)) return;
            if(objective != nextObjective) return;
            completedObjectives.Add(objective); 
            nextObjective = quest.GetNextObjective(completedObjectives.Count);
        }

        public bool IsComplete()
        {
            return nextObjective == "quest complete";
        }

        public object CaptureState()
        {
            return new QuestStatusRecord(quest.name, completedObjectives);
        }

        [System.Serializable]
        class QuestStatusRecord
        {
            public string questName;
            public List<string> completedObjectives;

            public QuestStatusRecord(string name, List<string> completed)
            {
                questName = name;
                completedObjectives = completed;
            }
        }
    }
}