using System;
using System.Collections;
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

        [Header("Game Settings")] [SerializeField]
        private int score = InitialScore;

        [Header("UI References")] [SerializeField]
        private float messageSpacing = 20f;

        [SerializeField] private GameObject textMessage;
        [SerializeField] private GameObject textMessagesContainer;
        [SerializeField] private GameObject playerTextMessagesContainer;
        [SerializeField] private TextMeshProUGUI scoreTextContainer;
        [SerializeField] private GameObject phoneNotification;
        [SerializeField] private GameObject phoneFrame;

        [Header("Game Content")] [SerializeField]
        private NpcMessage[] npcMessages;

        [Header("Notification Effects")] [SerializeField]
        private float flashSpeed = 1.5f;

        [SerializeField] private float minOpacity = 0.2f;
        [SerializeField] private float maxOpacity = 1.0f;
        [SerializeField] private float vibrateAmount = 5.0f;
        [SerializeField] private float vibrateDuration = 0.5f;
        private Coroutine _flashingCoroutine;
        private bool _isGameIndicationTriggered;

        private float _lastMessageBottomPosition;
        private string _nextTextMessage;
        private CanvasGroup _notificationCanvasGroup;
        private Vector3 _phoneOriginalPosition;
        private Coroutine _vibrateCoroutine;

        private void Start()
        {
            // CloseMiniGame();

            // Get or add CanvasGroup component to notification
            _notificationCanvasGroup = phoneNotification.GetComponent<CanvasGroup>();
            if (_notificationCanvasGroup == null)
                _notificationCanvasGroup = phoneNotification.AddComponent<CanvasGroup>();

            // Store original phone position
            if (phoneFrame != null) _phoneOriginalPosition = phoneFrame.transform.localPosition;
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
            switch (eventData.Name)
            {
                case GameEvents.PlayerClickOnChoice:
                {
                    if (!isGameActive)
                        return;

                    var choiceData = (EnrichedPlayerChoice)eventData.Data;
                    _nextTextMessage = choiceData.OriginalInteraction.DialogLine.voicedLines[0].text;
                    CreateTextMessage(choiceData.Choice, playerTextMessagesContainer);
                    Invoke(nameof(CreateTextMessageFromNext), 1.6f);
                    break;
                }
                case GameEvents.ThoughtScoreChange:
                {
                    if (!isGameActive)
                        return;

                    var newScore = (int)eventData.Data;
                    if (newScore != 0)
                    {
                        score += newScore;
                        scoreTextContainer.text = score.ToString();

                        // if (score is <= 0 or >= 100)
                        Invoke(nameof(CloseMiniGame), 3);
                    }

                    break;
                }

                case GameEvents.MiniGameIndicationTrigger:
                {
                    if ((MiniGameName)eventData.Data == MiniGameName.Reply)
                        TriggerMiniGameIndication();
                    else
                        StopMiniGameIndication();

                    break;
                }

                case GameEvents.TextMessageGameStart:
                {
                    if (_isGameIndicationTriggered)
                    {
                        StopMiniGameIndication();
                        StartMiniGame();
                    }

                    break;
                }

                case GameEvents.MiniGameClosed:
                    StopMiniGameIndication();
                    break;
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

        private void TriggerMiniGameIndication()
        {
            _isGameIndicationTriggered = true;
            phoneNotification.SetActive(true);
            StartFlashingNotification();
            StartPhoneVibration();
        }

        private void StopMiniGameIndication()
        {
            _isGameIndicationTriggered = false;
            phoneNotification.SetActive(false);
            StopAllCoroutines();
        }

        /// <summary>
        ///     Creates a flashing light effect by changing the opacity of the notification
        /// </summary>
        private void StartFlashingNotification()
        {
            // Stop any existing flashing coroutine
            if (_flashingCoroutine != null) StopCoroutine(_flashingCoroutine);

            // Start new flashing coroutine
            _flashingCoroutine = StartCoroutine(FlashNotificationCoroutine());
        }

        private IEnumerator FlashNotificationCoroutine()
        {
            float t = 0;

            while (true)
            {
                // Calculate opacity using a sine wave for smooth transitions
                var opacity = Mathf.Lerp(minOpacity, maxOpacity, (Mathf.Sin(t * flashSpeed) + 1) / 2);
                _notificationCanvasGroup.alpha = opacity;

                t += Time.deltaTime;
                yield return null;
            }
        }

        /// <summary>
        ///     Creates a vibration animation for the phone by slightly moving it
        /// </summary>
        private void StartPhoneVibration()
        {
            // Stop any existing vibration coroutine
            if (_vibrateCoroutine != null) StopCoroutine(_vibrateCoroutine);

            // Start new vibration coroutine
            _vibrateCoroutine = StartCoroutine(VibratePhoneCoroutine());
        }

        private IEnumerator VibratePhoneCoroutine()
        {
            var endTime = Time.time + vibrateDuration;

            while (Time.time < endTime)
            {
                // Create random offset for vibration effect
                var randomOffset = new Vector3(
                    Random.Range(-vibrateAmount, vibrateAmount),
                    Random.Range(-vibrateAmount, vibrateAmount),
                    0
                ) * 0.1f;

                // Apply offset to phone position
                phoneFrame.transform.localPosition = _phoneOriginalPosition + randomOffset;

                yield return new WaitForSeconds(0.05f);
            }

            // Reset phone position when vibration ends
            phoneFrame.transform.localPosition = _phoneOriginalPosition;
        }

        /// <summary>
        ///     Stops all notification effects
        /// </summary>
        private void StopNotificationEffects()
        {
            if (_flashingCoroutine != null)
            {
                StopCoroutine(_flashingCoroutine);
                _flashingCoroutine = null;
            }

            if (_vibrateCoroutine != null)
            {
                StopCoroutine(_vibrateCoroutine);
                _vibrateCoroutine = null;

                // Reset phone position
                if (phoneFrame != null) phoneFrame.transform.localPosition = _phoneOriginalPosition;
            }

            // Reset notification opacity
            if (_notificationCanvasGroup) _notificationCanvasGroup.alpha = 1.0f;
        }

        protected override void CloseMiniGame()
        {
            base.CloseMiniGame();
            StopNotificationEffects();
            phoneNotification.SetActive(false);
        }
    }
}