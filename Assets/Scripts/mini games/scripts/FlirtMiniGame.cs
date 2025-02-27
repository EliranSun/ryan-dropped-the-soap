using System;
using System.Linq;
using Dialog.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace mini_games.scripts
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
        public int timer = 5;
        [SerializeField] private ActorName actorName = ActorName.Morgan;
        [SerializeField] private FlirtChoice[] choices;
        [SerializeField] private SpriteEmotion[] actorSpriteEmotions;

        [SerializeField] private TextMeshProUGUI scoreTextContainer;
        [SerializeField] private TextMeshProUGUI timerTextContainer;
        [SerializeField] private Image characterImageContainer;
        [SerializeField] private GameObject miniGameContainer;
        [SerializeField] private GameObject inGameTrigger;
        private float _currentTime;
        private bool _isTimerRunning;


        private void Start()
        {
            CloseMiniGame();
            Invoke(nameof(StartMiniGame), 5);
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
            }
        }

        public void OnNotify(GameEventData eventData)
        {
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
            miniGameContainer.SetActive(false);
            Notify(GameEvents.KillThoughtsAndSayings);
        }

        private void StartMiniGame()
        {
            miniGameContainer.SetActive(true);
            Notify(GameEvents.AddThoughts, new ThoughtChoice { flirtChoices = choices });
            scoreTextContainer.text = score.ToString();

            // Initialize and start the timer
            _currentTime = timer;
            timerTextContainer.text = timer.ToString();
            _isTimerRunning = true;
        }

        public void SetActorName(ActorName actorName)
        {
            this.actorName = actorName;
        }
    }
}