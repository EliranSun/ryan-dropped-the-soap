using System;
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
    }

    [Serializable]
    public class PartnerChoice
    {
        public Sprite initPartnerExpression;
        public string choiceText;
        public AudioClip choiceAudio;
        public PlayerResponse[] playerResponses;
    }

    public class AttendPartnerMiniGame : MonoBehaviour
    {
        [SerializeField] private Sprite partnerBody;
        [SerializeField] private TextMeshProUGUI partnerRequest;
        [SerializeField] private Button choiceA;
        [SerializeField] private Button choiceB;
        [SerializeField] private Image partnerBodyImage;
        [SerializeField] private Image partnerExpressionImage;
        [SerializeField] private PartnerChoice[] partnerChoices;

        private void Start()
        {
            partnerBodyImage.sprite = partnerBody;
            if (partnerChoices == null || partnerChoices.Length == 0) return;

            var choice = partnerChoices[Random.Range(0, partnerChoices.Length)];

            partnerRequest.text = choice.choiceText;
            if (choice.initPartnerExpression != null)
                partnerExpressionImage.sprite = choice.initPartnerExpression;

            if (choice.playerResponses == null || choice.playerResponses.Length == 0) return;
            SetButtonText(choiceA, choice.playerResponses[0].response);

            if (choice.playerResponses.Length > 1)
                SetButtonText(choiceB, choice.playerResponses[1].response);
        }

        private static void SetButtonText(Button button, string text)
        {
            if (!button) return;

            var label = button.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) label.text = text;
        }
    }
}