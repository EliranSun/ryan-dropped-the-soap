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

    public class TextMessagesMiniGame : ObserverSubject
    {
        private const float MessageSpacing = 20f;
        [SerializeField] private GameObject textMessage;
        [SerializeField] private GameObject miniGameContainer;
        [SerializeField] private GameObject textMessagesContainer;
        [SerializeField] private NpcMessage[] npcMessages;

        private bool _isGameActive;
        private float _lastMessageBottomPosition;

        private void Start()
        {
            miniGameContainer.SetActive(false);
            miniGameContainer.SetActive(true);
            _isGameActive = true;
            CreateNpcMessage();
            CreateNpcMessage("?");
        }

        /// <summary>
        ///     Creates a random NPC message and positions it in the mini-game container
        /// </summary>
        private void CreateNpcMessage()
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
            CreateNpcMessage(selectedMessage.text, selectedMessage.choices);
        }

        /// <summary>
        ///     Creates an NPC message with the specified text and positions it in the mini-game container
        /// </summary>
        /// <param name="messageText">The text to display in the message</param>
        /// <param name="choices"></param>
        private void CreateNpcMessage(string messageText, PlayerMiniGameChoice[] choices = null)
        {
            // Instantiate the text message prefab
            var messageInstance = Instantiate(textMessage, textMessagesContainer.transform);

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
                position.y = _lastMessageBottomPosition - messageHeight - MessageSpacing;

                // Adjust x position based on the -20-degree angle of the phone screen
                // As messages go down (negative y), they should shift left (negative x)
                var angleInRadians = -20f * Mathf.Deg2Rad;
                position.x -= position.y * Mathf.Tan(angleInRadians);

                messageRect.localPosition = position;

                // Update the last message position for the next message
                _lastMessageBottomPosition = position.y - messageHeight;
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
            if (!_isGameActive)
                return;

            if (eventData.Name == GameEvents.PlayerClickOnChoice)
            {
                var choiceData = (EnrichedPlayerChoice)eventData.Data;
                print("Text message mini game choice:" + choiceData.Choice);
                CreateNpcMessage(choiceData.Choice);
            }
        }
    }
}