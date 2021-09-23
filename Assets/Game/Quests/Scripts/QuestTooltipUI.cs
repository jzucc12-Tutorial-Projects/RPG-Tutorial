using RPG.Quests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Quests
{
    public class QuestTooltipUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI title = null;
        [SerializeField] Transform objectiveContainer = null;
        [SerializeField] GameObject objectivePrefab = null;
        [SerializeField] GameObject objectiveIncompletePrefab = null;
        [SerializeField] TextMeshProUGUI rewardText = null;

        public void Setup(QuestStatus status)
        {
            Quest quest = status.GetQuest();
            foreach(HorizontalLayoutGroup obj in objectiveContainer.GetComponentsInChildren<HorizontalLayoutGroup>())
                Destroy(obj.gameObject);

            title.text = quest.GetName();
            foreach(Objective objective in quest.GetObjectives())
            {
                GameObject prefab;
                if (status.IsObjectiveComplete(objective.reference))
                    prefab = Instantiate(objectivePrefab, objectiveContainer);
                else
                    prefab = Instantiate(objectiveIncompletePrefab, objectiveContainer);
                    
                prefab.GetComponentInChildren<TextMeshProUGUI>().text = objective.description;
            } 

            rewardText.text = GetRewardText(quest);
        }

        string GetRewardText(Quest quest)
        {
            string output = "";
            foreach(Reward reward in quest.GetRewards())
            {
                string num = "";
                if(reward.number > 1)
                    num = reward.number.ToString() + " ";
                output += num + reward.item.name + ", ";
            }

            if(string.IsNullOrEmpty(output))
                return "No reward";
            else
                return output.Remove(output.Length - 2);
        }
    }
}
