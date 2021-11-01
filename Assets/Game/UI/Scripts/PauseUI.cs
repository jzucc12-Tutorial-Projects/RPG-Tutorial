using RPG.Control;
using RPG.SceneManagement;
using UnityEngine;

namespace RPG.UI
{
    public class PauseUI : MonoBehaviour
    {
        PlayerController playerController;

        private void Awake() 
        {
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();    
        }

        private void OnEnable() 
        {
            if(!playerController) return;
            Time.timeScale = 0;
            playerController.enabled = false;
        }

        private void OnDisable() 
        {
            if(!playerController) return;
            Time.timeScale = 1;
            playerController.enabled = true;
        }

        public void Save()
        {
            FindObjectOfType<SavingWrapper>().Save();
        }

        public void SaveAndQuit()
        {
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            savingWrapper.Save();
            savingWrapper.LoadMenu();
        }
    }
}
