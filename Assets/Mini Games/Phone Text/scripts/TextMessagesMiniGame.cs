using System;
using Dialog.Scripts;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mini_Games.Phone_Text.scripts
{
    [Serializable]
    public class NpcMessage
    {
        public string text;
        public PlayerMiniGameChoice[] choices;
    }

    public class TextMessagesMiniGame : MiniGame
    {
        private const int InitialScore = -50;
        [SerializeField] private int score = InitialScore;
        [SerializeField] private float messageSpacing = 20f;
        [SerializeField] private GameObject textMessage;
        [SerializeField] private GameObject textMessagesContainer;
        [SerializeField] private GameObject playerTextMessagesContainer;
        [SerializeField] private TextMeshProUGUI scoreTextContainer;
        [SerializeField] private NpcMessage[] npcMessages;

        private float _lastMessageBottomPosition;
        private string _nextTextMessage;

        private void Start()
        {
            CloseMiniGame();
        }

        private void CreateRandomTextMessage(GameObject container)
        {
            if (npcMessages == null || npcMessages.Length == 0)
            {
                Debug.LogWarning("No NPC messages available to display");
                return;
            }

            // Randomly select a message
            var randomIndex = Random.Range(0, npcMessages.Length);
            var selectedMessage = npcMessages[randomIndex];

            // Create message with the selected text
            CreateTextMessage(selectedMessage.text, container, selectedMessage.choices);
        }

        private void CreateTextMessage(string messageText, GameObject container, PlayerMiniGameChoice[] choices = null)
        {
            // Instantiate the text message prefab
            var messageInstance = Instantiate(textMessage, container.transform);

            // Find the TextMeshPro component for the message text
            var textTransform = messageInstance.transform.Find("text");
            if (textTransform != null)
            {
                var textComponent = textTransform.GetComponent<TextMeshProUGUI>();
                if (textComponent != null)
                    textComponent.text = messageText;
                else
                    Debug.LogError("TextMeshProUGUI component not found on 'text' child");
            }
            else
            {
                Debug.LogError("Child 'text' not found in text message prefab");
            }

            // Position the message below the last message
            var messageRect = messageInstance.GetComponent<RectTransform>();
            if (messageRect != null)
            {
                // Calculate the height of the current message
                var messageHeight = messageRect.rect.height * messageRect.localScale.y;

                // Position the message below the last message
                var position = messageRect.localPosition;
                position.y = _lastMessageBottomPosition - messageHeight + messageSpacing;

                // Adjust x position based on the -20-degree angle of the phone screen
                // As messages go down (negative y), they should shift left (negative x)
                var angleInRadians = -20f * Mathf.Deg2Rad;
                position.x -= position.y * Mathf.Tan(angleInRadians);

                messageRect.localPosition = position;

                // Update the last message position for the next message
                _lastMessageBottomPosition = position.y - messageHeight + messageSpacing;
            }
            else
            {
                Debug.LogError("RectTransform component not found on text message prefab");
            }

            if (choices is { Length: > 0 })
                Notify(GameEvents.AddThoughts, new ThoughtChoice
                {
                    choices = choices
                });
        }

        public void OnNotify(GameEventData eventData)
        {
            if (!isGameActive)
                return;

            if (eventData.Name == GameEvents.PlayerClickOnChoice)
            {
                var choiceData = (EnrichedPlayerChoice)eventData.Data;
                _nextTextMessage = choiceData.OriginalInteraction.DialogLine.voicedLines[0].text;
                CreateTextMessage(choiceData.Choice, playerTextMessagesContainer);
                Invoke(nameof(CreateTextMessageFromNext), 1.6f);
            }

            if (eventData.Name == GameEvents.ThoughtScoreChange)
            {
                var newScore = (int)eventData.Data;
                if (newScore != 0)
                {
                    score += newScore;
                    scoreTextContainer.text = score.ToString();

                    // if (score is <= 0 or >= 100)
                    Invoke(nameof(CloseMiniGame), 3);
                }
            }
        }

        protected override void StartMiniGame()
        {
            base.StartMiniGame(); // Call the base class method to start the timer

            isGameActive = true;
            miniGameContainer.SetActive(true);

            scoreTextContainer.text = score.ToString();

            if (score == InitialScore) // first time open
            {
                CreateRandomTextMessage(textMessagesContainer);
                CreateTextMessage("?", textMessagesContainer);
            }
        }

        private void CreateTextMessageFromNext()
        {
            if (_nextTextMessage == "")
                return;

            CreateTextMessage(_nextTextMessage, textMessagesContainer);
        }
    }
}