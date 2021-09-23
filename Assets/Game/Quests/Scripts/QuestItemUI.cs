using UnityEngine;
using RPG.Quests;
using TMPro;

namespace RPG.UI.Quests
{
    public class QuestItemUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI title = null;
        [SerializeField] TextMeshProUGUI progress = null;
        QuestStatus status;

        public void Setup(QuestStatus status)
        {
            this.status = status;
            title.text = status.GetQuest().GetName();
            progress.text = status.GetCompletedCount() + "/" + status.GetQuest().GetObjectiveCount();
        }

        public QuestStatus GetQuestStatus() { return status; }
    }
}