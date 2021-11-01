using RPG.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class TraitRowUI : MonoBehaviour
    {
        [SerializeField] Trait trait;
        [SerializeField] TextMeshProUGUI valueText = null;
        [SerializeField] Button plusButton = null;
        [SerializeField] Button minusButton = null;
        TraitStore traitStore;

        private void Start() 
        {
            traitStore = GameObject.FindGameObjectWithTag("Player").GetComponent<TraitStore>();
            plusButton.onClick.AddListener(() => Allocate(1));
            minusButton.onClick.AddListener(() => Allocate(-1));
        }

        private void Update() 
        {
            minusButton.interactable = traitStore.CanAssignPoints(trait, -1);
            plusButton.interactable = traitStore.CanAssignPoints(trait, 1);
            valueText.text = traitStore.GetProposedPoints(trait).ToString();
        }

        public void Allocate(int points)
        {
            traitStore.AssignPoints(trait, points);
        }
    }
}