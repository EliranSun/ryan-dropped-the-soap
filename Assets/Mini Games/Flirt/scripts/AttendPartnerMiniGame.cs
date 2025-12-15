using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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

    public class AttendPartnerMiniGame : ObserverSubject
    {
        [SerializeField] private MiniGameName miniGameName;
        [SerializeField] private Sprite partnerBody;
        [SerializeField] private TextMeshProUGUI partnerRequest;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private GameObject gameContainer;

        [FormerlySerializedAs("choiceA")] [SerializeField]
        private Button choiceAButton;

        [FormerlySerializedAs("choiceB")] [SerializeField]
        private Button choiceBButton;

        [SerializeField] private Image partnerBodyImage;
        [SerializeField] private Image partnerExpressionImage;
        [SerializeField] private PartnerChoice[] partnerChoices;

        private PartnerChoice _currentChoice;

        private void Start()
        {
            gameContainer.SetActive(false);
        }

        private static void SetButtonText(Button button, string text)
        {
            if (!button) return;

            var label = button.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) label.text = text;
        }

        public void OnChoiceA()
        {
            ChangePartnerResponse(0);
        }

        public void OnChoiceB()
        {
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
                    partnerRequest.text = response.isAttentive ? "!!!" : "...";

                    var isWinState = (miniGameName == MiniGameName.Neglect && !response.isAttentive) ||
                                     (miniGameName == MiniGameName.Attend && response.isAttentive);

                    if (isWinState)
                        Notify(GameEvents.MiniGameWon, 1);
                    else
                        Notify(GameEvents.MiniGameLost, 0);

                    Invoke(nameof(EndGame), 2);
                }
            }
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.MiniGameIndicationTrigger)
            {
                var eventMiniGameName = (MiniGameName)eventData.Data;
                if (eventMiniGameName == miniGameName) StartGame();
            }
        }

        private void EndGame()
        {
            gameContainer.SetActive(false);
        }

        private void StartGame()
        {
            gameContainer.SetActive(true);
            partnerBodyImage.sprite = partnerBody;
            if (partnerChoices == null || partnerChoices.Length == 0) return;

            _currentChoice = partnerChoices[Random.Range(0, partnerChoices.Length)];

            partnerRequest.text = _currentChoice.choiceText;
            if (audioSource)
            {
                audioSource.clip = _currentChoice.choiceAudio;
                audioSource.Play();
            }

            if (_currentChoice.initPartnerExpression != null)
                partnerExpressionImage.sprite = _currentChoice.initPartnerExpression;

            if (_currentChoice.playerResponses == null || _currentChoice.playerResponses.Length == 0)
                return;

            Button[] choiceButtons = { choiceAButton, choiceBButton };
            for (var i = 0; i < _currentChoice.playerResponses.Length && i < choiceButtons.Length; i++)
                SetButtonText(choiceButtons[i], _currentChoice.playerResponses[i].response);
        }
    }
}