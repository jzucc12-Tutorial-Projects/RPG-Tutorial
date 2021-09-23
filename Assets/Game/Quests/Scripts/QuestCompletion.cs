using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestCompletion : MonoBehaviour
    {
        [SerializeField] Quest quest = null;
        [SerializeField] string objectiveRef = "";


        public void CompleteObjective()
        {
            var questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>(); 
            if (!questList.HasQuest(quest)) return;  
            questList.CompleteObjective(quest, objectiveRef);
        }
    }
}
