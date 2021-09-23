using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;

namespace RPG.Quests
{
    [CreateAssetMenu(fileName = "Quest", menuName = "RPG Tutorial/Quest", order = 0)]
    public class Quest : ScriptableObject 
    {
        [SerializeField] List<Objective> objectives = new List<Objective>();
        [SerializeField] List<Reward> rewards = new List<Reward>();

        public string GetName() { return name; }
        public int GetObjectiveCount() { return objectives.Count; }
        public IEnumerable<Objective> GetObjectives() { return objectives; }
        public IEnumerable<Reward> GetRewards() { return rewards; } 
        public string GetNextObjective(int id)
        {
            if(id >= objectives.Count) return "quest complete";
            return objectives[id].reference;
        }
        public bool HasObjective(string objectiveRef)
        {
            foreach(var objective in objectives)
            {
                if(objective.reference != objectiveRef) continue;
                return true;
            }
            return false;
        }

        public static Quest GetByName(string questName)
        {
            foreach(Quest quest in Resources.LoadAll<Quest>(""))
            {
                if(quest.name != questName) continue;
                return quest;
            }

            return null;
        }


    }

    [System.Serializable]
    public class Reward
    {
        [Min(1)] public int number;
        public InventoryItem item;
    }

    [System.Serializable]
    public class Objective
    {
        public string reference;
        public string description;
    }
}