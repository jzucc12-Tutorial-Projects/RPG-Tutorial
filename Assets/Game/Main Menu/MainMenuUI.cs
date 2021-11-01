using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using RPG.SceneManagement;
using UnityEngine;
using TMPro;

namespace RPG.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        LazyValue<SavingWrapper> savingWrapper;

        private void Awake()
        {
            savingWrapper = new LazyValue<SavingWrapper>(GetSavingWrapper);
        }

        SavingWrapper GetSavingWrapper()
        {
            return FindObjectOfType<SavingWrapper>();
        }

        public void ContinueGame()
        {
            savingWrapper.value.ContinueGame();
        }

        public void NewGame()
        {
            string name = GetComponentInChildren<TMP_InputField>().text;
            savingWrapper.value.NewGame(name);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}