using System;
using System.Linq;
using Character_Creator.scripts;
using Dialog.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Mini_Games.Flirt.scripts
{
    [Serializable]
    internal class SpriteEmotion
    {
        public Sprite sprite;
        public Reaction reaction;
    }


    public class FlirtMiniGame : ObserverSubject
    {
        public int score = 50;
        [SerializeField] private int timer = 8;
        [SerializeField] private ActorName actorName = ActorName.Morgan;
        [SerializeField] private PlayerMiniGameChoice[] choices;
        [SerializeField] private SpriteEmotion[] actorSpriteEmotions;

        [SerializeField] private TextMeshProUGUI scoreTextContainer;
        [SerializeField] private TextMeshProUGUI timerTextContainer;
        [SerializeField] private Image characterImageContainer;
        [SerializeField] private GameObject miniGameContainer;
        [SerializeField] private GameObject inGameTrigger;
        private float _currentTime;
        private bool _isGameActive;
        private bool _isTimerRunning;


        private void Start()
        {
            CloseMiniGame();
        }

        private void Update()
        {
            if (_isTimerRunning) UpdateTimer();
        }

        private void UpdateTimer()
        {
            _currentTime -= Time.deltaTime;

            // Update the timer display
            var timeRemaining = Mathf.CeilToInt(_currentTime);
            timerTextContainer.text = timeRemaining.ToString();

            // Check if timer has reached zero
            if (_currentTime <= 0)
            {
                _isTimerRunning = false;
                CloseMiniGame();
                Notify(GameEvents.KillThoughtsAndSayings);
                Notify(GameEvents.KillDialog);
            }
        }

        public void OnNotify(GameEventData eventData)
        {
            if (!_isGameActive)
                return;

            print("Flirt OnNotify: " + eventData.Data);
            if (eventData.Name == GameEvents.ClickOnNpc)
            {
                var interactionData = eventData.Data as InteractionData;
                if (interactionData == null) return;

                if (interactionData.Name == inGameTrigger.gameObject.name)
                    StartMiniGame();
            }

            if (eventData.Name == GameEvents.FlirtGameStart)
            {
                CloseMiniGame();
                StartMiniGame();
            }

            if (eventData.Name == GameEvents.ActorReaction)
            {
                var dialogLine = eventData.Data as NarrationDialogLine;

                if (!dialogLine) return;
                if (dialogLine.actorName != actorName) return;

                var reaction = actorSpriteEmotions.First(x =>
                    x.reaction == dialogLine.actorReaction);

                characterImageContainer.sprite = reaction.sprite;
            }

            if (eventData.Name == GameEvents.ThoughtScoreChange)
            {
                var newScore = (int)eventData.Data;
                if (newScore != 0)
                {
                    score += newScore;
                    scoreTextContainer.text = score.ToString();

                    if (score is <= 0 or >= 100)
                        Invoke(nameof(CloseMiniGame), 2);
                }
            }
        }

        private void CloseMiniGame()
        {
            _isTimerRunning = false;
            _isGameActive = false;
            miniGameContainer.SetActive(false);
        }

        private void StartMiniGame()
        {
            miniGameContainer.SetActive(true);
            _isGameActive = true;

            // Randomly select 4 choices from the available choices
            var randomChoices = GetRandomChoices(choices, 4);

            Notify(GameEvents.AddThoughts, new ThoughtChoice
            {
                choices = randomChoices
            });
            scoreTextContainer.text = score.ToString();

            // Initialize and start the timer
            _currentTime = timer;
            timerTextContainer.text = timer.ToString();
            _isTimerRunning = true;
        }

        // Helper method to randomly select n choices from the available choices
        private PlayerMiniGameChoice[] GetRandomChoices(PlayerMiniGameChoice[] availableChoices, int count)
        {
            // Make sure we don't try to select more choices than available
            count = Mathf.Min(count, availableChoices.Length);

            // Create a copy of the available choices to avoid modifying the original array
            var choicesCopy = availableChoices.ToArray();

            // Shuffle the array using Fisher-Yates algorithm
            for (var i = choicesCopy.Length - 1; i > 0; i--)
            {
                var randomIndex = Random.Range(0, i + 1);
                (choicesCopy[i], choicesCopy[randomIndex]) = (choicesCopy[randomIndex], choicesCopy[i]);
            }

            // Return the first 'count' elements from the shuffled array
            return choicesCopy.Take(count).ToArray();
        }
    }
}