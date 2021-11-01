using RPG.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class SaveLoadUI : MonoBehaviour
    {
        [SerializeField] Transform contentRoot = null;
        [SerializeField] GameObject buttonPrefab = null;

        private void OnEnable()
        {
            foreach(Transform child in contentRoot)
            {
                Destroy(child.gameObject);
            }

            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            foreach(string name in savingWrapper.ListSaves())
            {
                GameObject button = Instantiate(buttonPrefab, contentRoot);
                button.GetComponentInChildren<TextMeshProUGUI>().text = name;
                button.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    savingWrapper.LoadGame(name);
                });
            }
        }
    }
}