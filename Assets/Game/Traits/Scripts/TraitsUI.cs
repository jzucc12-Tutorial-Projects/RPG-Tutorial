using RPG.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class TraitsUI : MonoBehaviour
    {
        [SerializeField] Button confirmButton = null;
        [SerializeField] TextMeshProUGUI unassignedPoints = null;
        TraitStore traitStore;

        private void Start() 
        {
            traitStore = GameObject.FindGameObjectWithTag("Player").GetComponent<TraitStore>();
            confirmButton.onClick.AddListener(traitStore.Commit);
        }

        private void Update() 
        {
            unassignedPoints.text = traitStore.GetUnassignedPoints().ToString();
        }
    }
}