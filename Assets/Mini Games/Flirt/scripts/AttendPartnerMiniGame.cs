using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
            partnerRequest.text = partnerChoices[Random.Range(0, partnerChoices.Length)].choiceText;
        }
    }
}