using TMPro;
using UnityEngine;

namespace Mini_Games
{
    public class MiniGame : ObserverSubject
    {
        [Header("Mini Game Settings")] [SerializeField]
        private TextMeshProUGUI timerTextContainer;

        [SerializeField] private int timer = 8;
        [SerializeField] public GameObject miniGameContainer;

        public bool isGameActive;

        private float _currentTime;
        private bool _isTimerRunning;

        private void Update()
        {
            if (_isTimerRunning) UpdateTimer();
        }

        protected virtual void UpdateTimer()
        {
            _currentTime -= Time.deltaTime;

            // Update the timer display
            var timeRemaining = Mathf.CeilToInt(_currentTime);
            timerTextContainer.text = timeRemaining.ToString();

            // Check if timer has reached zero
            if (_currentTime <= 0) CloseMiniGame();
        }

        protected virtual void StartMiniGame()
        {
            _currentTime = timer;
            timerTextContainer.text = timer.ToString();
            _isTimerRunning = true;
        }

        protected virtual void CloseMiniGame()
        {
            _isTimerRunning = false;
            isGameActive = false;
            miniGameContainer.SetActive(false);
            Notify(GameEvents.KillThoughtsAndSayings);
            Notify(GameEvents.KillDialog);
            Notify(GameEvents.MiniGameClosed);
        }
    }
}