using System.Collections;
using UnityEngine;
using GameDevTV.Saving;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] float fadeInTime = 0.2f;
        [SerializeField] float fadeOutTime = 0.2f;
        [SerializeField] int firstScene = 1;
        [SerializeField] int menuScene = 0;
        string saveNameKey = "currentSaveName";
        

        public void ContinueGame()
        {
            if(!PlayerPrefs.HasKey(saveNameKey)) return;
            if(!GetComponent<SavingSystem>().SaveFileExists(GetSaveName())) return;
            StartCoroutine(LoadLastScene());
        }

        public void NewGame(string saveFile)
        {
            if(string.IsNullOrEmpty(saveFile)) return;
            SetSaveName(saveFile);
            StartCoroutine(LoadFirstScene());
        }

        public void LoadGame(string saveFile)
        {
            if(string.IsNullOrEmpty(saveFile)) return;
            SetSaveName(saveFile);
            if(!GetComponent<SavingSystem>().SaveFileExists(GetSaveName())) return;
            StartCoroutine(LoadLastScene());
        }

        IEnumerator LoadLastScene()
        {
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);
            yield return GetComponent<SavingSystem>().LoadLastScene(GetSaveName());
            yield return fader.FadeIn(fadeInTime);
        }

        IEnumerator LoadFirstScene()
        {
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);
            yield return SceneManager.LoadSceneAsync(firstScene);
            yield return fader.FadeIn(fadeInTime);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            if(Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(GetSaveName());
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(GetSaveName());
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(GetSaveName());
        }

        string GetSaveName()
        {
            return PlayerPrefs.GetString(saveNameKey);
        }

        void SetSaveName(string name)
        {
            PlayerPrefs.SetString(saveNameKey, name);
        }

        public IEnumerable<string> ListSaves()
        {
            return GetComponent<SavingSystem>().ListSaves();
        }

        public void LoadMenu()
        {
            StartCoroutine(LoadMenuRoutine());
        }

        IEnumerator LoadMenuRoutine()
        {
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);
            yield return SceneManager.LoadSceneAsync(menuScene);
            yield return fader.FadeIn(fadeInTime);
        }
    }
}
