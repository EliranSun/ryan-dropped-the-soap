using Character_Creator.scripts;
using TMPro;
using UnityEngine;

namespace Mini_Games
{
    public class MiniGame : ObserverSubject
    {
        [Header("Mini Game Settings")] [SerializeField]
        private TextMeshProUGUI timerTextContainer;

        [SerializeField] private int timer = 60;
        [SerializeField] public GameObject miniGameContainer;
        [SerializeField] private GameObject hideOnStart;
        [SerializeField] public GameObject inGameTrigger;

        public bool isGameActive;

        private float _currentTime;
        private bool _isTimerRunning;

        private void Update()
        {
            if (_isTimerRunning)
                UpdateTimer();
        }

        protected virtual void UpdateTimer()
        {
            _currentTime -= Time.deltaTime;

            // Update the timer display
            var timeRemaining = Mathf.CeilToInt(_currentTime);
            if (timerTextContainer) timerTextContainer.text = timeRemaining.ToString();

            // Check if timer has reached zero
            if (_currentTime <= 0)
                CloseMiniGame();
        }

        protected virtual void StartMiniGame()
        {
            _currentTime = timer;
            _isTimerRunning = true;

            if (timerTextContainer) timerTextContainer.text = timer.ToString();
            if (miniGameContainer) miniGameContainer.SetActive(true);
            if (hideOnStart) hideOnStart.SetActive(false);

            Notify(GameEvents.MiniGameStart);
        }

        protected virtual void CloseMiniGame()
        {
            _isTimerRunning = false;
            isGameActive = false;

            if (miniGameContainer) miniGameContainer.SetActive(false);
            if (hideOnStart) hideOnStart.SetActive(true);

            Notify(GameEvents.KillThoughtsAndSayings);
            Notify(GameEvents.KillDialog);
            Notify(GameEvents.MiniGameClosed);
        }

        public virtual void OnNotify(GameEventData eventData)
        {
            switch (eventData.Name)
            {
                case GameEvents.ClickOnNpc:
                    var interactionData = eventData.Data as InteractionData;
                    if (interactionData == null) return;

                    if (interactionData.Name == inGameTrigger.gameObject.name)
                        StartMiniGame();
                    break;
            }
        }
    }
}