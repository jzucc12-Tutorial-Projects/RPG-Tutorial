using UnityEngine;
using RPG.Quests;

namespace RPG.UI.Quests
{
    public class QuestListUI : MonoBehaviour
    {
        QuestList questList;
        [SerializeField] QuestItemUI questPrefab = null;

        private void Start()
        {
            questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            questList.ListUpdated += UpdateUI;
            UpdateUI();
        }

        private void UpdateUI()
        {
            foreach (QuestItemUI quest in GetComponentsInChildren<QuestItemUI>())
                Destroy(quest.gameObject);

            foreach (QuestStatus status in questList.GetStatuses())
            {
                var prefab = Instantiate(questPrefab, transform);
                prefab.Setup(status);
            }
        }
    }
}
