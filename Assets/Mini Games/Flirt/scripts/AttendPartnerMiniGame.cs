using System;
using System.Collections;
using interactions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/*
 * Let's go on a date - NO | OK
 * Give me a kiss - AVOID | OK
 * Give me a hug - AVOID | OK
 * Should we try for a kid? - AVOID | OK
 * Let's have sex  - AVOID | OK
 * Do you even love me anymore? - NO | YES
 * We don't make love anymore... - SO | OK
 * Are you cheating on me? - NO | YES
 * What do you want for dinner? DUNNO | DONT CARE
 * Can you take out the trash? AVOID | OK
 * What should we do for my birthday? - DUNNO | DONT CARE
 * It's our anniversary tomorrow... - DUNNO | DONT CARE
 * Let's do something nice - AVOID | OK
 * Do I look nice in this dress? DUNNO | DONT CARE
 * You never say nice things to me - SO | OK
 * What happened to us? AVOID | DUNNO
 * Hold my hand - AVOID | OK
 * Can you hold me? - AVOID | OK
 * I am depressed - SO | OK
 */
namespace Mini_Games.Flirt.scripts
{
    [Serializable]
    public class PlayerResponse
    {
        public string response;
        public Sprite partnerResponseExpression;
        public Sprite responseResultImage;
        public bool isAttentive;
    }

    [Serializable]
    public class PartnerChoice
    {
        public Sprite initPartnerExpression;
        public string choiceText;
        public AudioClip choiceAudio;
        public PlayerResponse[] playerResponses;
    }

    [RequireComponent(typeof(AudioSource))]
    public class AttendPartnerMiniGame : ObserverSubject
    {
        [SerializeField] private MiniGameName miniGameName;
        [SerializeField] private Sprite partnerBody;
        [SerializeField] private TextMeshProUGUI partnerRequest;
        [SerializeField] private GameObject gameContainer;

        [SerializeField] private Button choiceAButton;

        [SerializeField] private Button choiceBButton;

        [SerializeField] private GameObject playerWrapper;
        [SerializeField] private Image partnerBodyImage;
        [SerializeField] private Image partnerExpressionImage;
        [SerializeField] private Image responseResultImageContainer;
        [SerializeField] private PartnerChoice[] partnerChoices;
        private AudioSource _audioSource;

        private PartnerChoice _currentChoice;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            gameContainer.SetActive(false);
            ToggleChoiceButtons(false);
            responseResultImageContainer.gameObject.SetActive(false);
        }

        private static void SetButtonText(Button button, string text)
        {
            if (!button) return;

            var label = button.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) label.text = text;
        }

        private void ToggleChoiceButtons(bool isEnabled)
        {
            choiceAButton.gameObject.SetActive(isEnabled);
            choiceBButton.gameObject.SetActive(isEnabled);
        }

        public void OnChoiceA()
        {
            print("CLICK ON CHOICE A");
            ChangePartnerResponse(0);
        }

        public void OnChoiceB()
        {
            print("CLICK ON CHOICE B");
            ChangePartnerResponse(1);
        }

        private void ChangePartnerResponse(int choiceIndex)
        {
            if (_currentChoice is { playerResponses: { Length: > 0 } })
            {
                // For choice A, use the first player response (index 0)
                var response = _currentChoice.playerResponses[choiceIndex];
                if (response != null && response.partnerResponseExpression != null)
                {
                    partnerExpressionImage.sprite = response.partnerResponseExpression;
                    StartCoroutine(ShowResponseResults(response.responseResultImage, 1));
                    partnerRequest.text = response.isAttentive ? "!!!" : "...";

                    var isWinState =
                        (miniGameName == MiniGameName.Neglect && !response.isAttentive) ||
                        (miniGameName == MiniGameName.Attend && response.isAttentive);

                    if (isWinState)
                        Notify(GameEvents.MiniGameWon, 1);
                    else
                        Notify(GameEvents.MiniGameLost, 0);

                    Invoke(nameof(EndGame), response.responseResultImage ? 4 : 2);
                }
            }
        }

        private IEnumerator ShowResponseResults(Sprite responseResult, float waitFor)
        {
            if (!responseResult || !responseResultImageContainer) yield break;

            yield return new WaitForSeconds(waitFor);

            ToggleChoiceButtons(false);

            partnerBodyImage.gameObject.SetActive(false);
            partnerExpressionImage.gameObject.SetActive(false);

            playerWrapper.gameObject.SetActive(false);

            responseResultImageContainer.gameObject.SetActive(true);
            responseResultImageContainer.sprite = responseResult;
        }

        public void OnNotify(GameEventData eventData)
        {
            // TEMP FOR TESTING: Should be handled via games manager
            if (eventData.Name == GameEvents.PlayerInteractionRequest)
            {
                var request = (ActorInteraction)eventData.Data;
                if (request.type == ObjectInteractionType.Attend) StartGame();
            }

            if (eventData.Name == GameEvents.MiniGameIndicationTrigger)
            {
                var eventMiniGameName = (MiniGameName)eventData.Data;
                if (eventMiniGameName == miniGameName) StartGame();
            }
        }

        private void EndGame()
        {
            ToggleChoiceButtons(false);
            gameContainer.SetActive(false);
        }

        private void StartGame()
        {
            if (partnerChoices == null || partnerChoices.Length == 0)
                return;

            gameContainer.SetActive(true);
            partnerBodyImage.sprite = partnerBody;

            _currentChoice = partnerChoices[Random.Range(0, partnerChoices.Length)];

            partnerRequest.text = _currentChoice.choiceText;

            if (_audioSource)
            {
                _audioSource.clip = _currentChoice.choiceAudio;
                _audioSource.Play();
            }

            if (_currentChoice.initPartnerExpression != null)
                partnerExpressionImage.sprite = _currentChoice.initPartnerExpression;

            if (_currentChoice.playerResponses == null || _currentChoice.playerResponses.Length == 0)
                return;

            Button[] choiceButtons = { choiceAButton, choiceBButton };
            for (var i = 0; i < _currentChoice.playerResponses.Length && i < choiceButtons.Length; i++)
                SetButtonText(choiceButtons[i], _currentChoice.playerResponses[i].response);

            ToggleChoiceButtons(true);
        }
    }
}