using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Assets.Dialogues;
using TMPro;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour
    {
        PlayerConversant playerConversant;
        [SerializeField] TextMeshProUGUI aiText = null;
        [SerializeField] Button nextButton = null;
        [SerializeField] Button QuitButton = null;
        [SerializeField] Transform choiceRoot = null;
        [SerializeField] GameObject choicePrefab = null;
        [SerializeField] GameObject AIResponseRoot = null;
        [SerializeField] TextMeshProUGUI currentSpeaker = null;
        bool buttonIsNext = false;

        void Start()
        {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>(); 
            playerConversant.OnConversationUpdated += UpdateUI;
            QuitButton.onClick.AddListener(() => playerConversant.Quit());
            NextButtonChangeState(true);
            UpdateUI();
        }

        void Next()
        {
            playerConversant.Next();
        }

        void Quit()
        {
            playerConversant.Quit();
        }

        void UpdateUI()
        {
            gameObject.SetActive(playerConversant.IsActive());
            if (!playerConversant.IsActive()) return;
            AIResponseRoot.SetActive(!playerConversant.IsChoosing());
            choiceRoot.gameObject.SetActive(playerConversant.IsChoosing());
            currentSpeaker.text = playerConversant.GetCurrentSpeaker();

            if(playerConversant.IsChoosing())
            {
                BuildChoiceList();
            }
            else
            {
                aiText.text = playerConversant.GetText();
                NextButtonChangeState(playerConversant.HasNext());
            }
        }

        void NextButtonChangeState(bool makeNext)
        {
            if(buttonIsNext == makeNext) return;

            buttonIsNext = makeNext;
            if(makeNext)
            {
                nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next ->";
                nextButton.onClick.AddListener(Next);
                nextButton.onClick.RemoveListener(Quit);
            }
            else
            {
                nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Close";
                nextButton.onClick.AddListener(Quit);
                nextButton.onClick.RemoveListener(Next);
            }
        }

        private void BuildChoiceList()
        {
            foreach (Transform item in choiceRoot)
                Destroy(item.gameObject);

            foreach (DialogueNode choice in playerConversant.GetChoices())
            {
                GameObject choiceButton = Instantiate(choicePrefab, choiceRoot);
                choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = choice.GetText();
                Button button = choiceButton.GetComponentInChildren<Button>();
                button.onClick.AddListener(() =>
                {
                    playerConversant.SelectChoice(choice);
                });
            }
        }
    }
}
