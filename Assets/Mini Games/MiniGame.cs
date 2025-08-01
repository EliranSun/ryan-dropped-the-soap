using System;
using Character_Creator.scripts;
using Dialog;
using Mini_Games.Organize_Desk.scripts;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mini_Games
{
    public class MiniGame : ObserverSubject
    {
        [Header("Mini Game Settings")] [SerializeField]
        private TextMeshProUGUI timerTextContainer;

        [SerializeField] private int timer = 60;
        [SerializeField] public GameObject mainCamera;
        [SerializeField] public GameObject miniGameContainer;
        [SerializeField] private GameObject hideOnStart;
        [SerializeField] public GameObject inGameTrigger;

        [Header("Game dialog responses")] [SerializeField]
        public TypedDialogLine[] dialogLines;

        public int score;
        public bool isGameActive;

        private float _currentTime;
        private bool _isTimerRunning;

        private void Update()
        {
            if (_isTimerRunning)
                UpdateTimer();
        }

        public NarrationDialogLine GetRandomLine(DialogLineType type)
        {
            // Filter lines by type
            var filteredLines = Array.FindAll(dialogLines, line => line.type == type);

            // If no lines of this type, return null
            if (filteredLines.Length == 0)
                return null;

            // Return a random line of the specified type
            var randomIndex = Random.Range(0, filteredLines.Length);
            return filteredLines[randomIndex].dialogLine;
        }

        protected virtual void UpdateTimer()
        {
            _currentTime -= Time.deltaTime;

            var timeRemaining = Mathf.CeilToInt(_currentTime);
            if (timerTextContainer) timerTextContainer.text = timeRemaining.ToString();

            if (_currentTime <= 0)
                CloseMiniGame(score > 0);
        }

        protected virtual void StartMiniGame()
        {
            _currentTime = timer;
            _isTimerRunning = true;

            if (hideOnStart) hideOnStart.SetActive(false);
            if (timerTextContainer) timerTextContainer.text = timer.ToString();
            if (miniGameContainer)
            {
                miniGameContainer.SetActive(true);
                var newPosition = mainCamera.transform.position;
                newPosition.z = miniGameContainer.transform.position.z;
                miniGameContainer.transform.position = newPosition;
            }


            Notify(GameEvents.MiniGameStart);
        }

        protected virtual void CloseMiniGame(bool isGameWon = false)
        {
            _isTimerRunning = false;
            isGameActive = false;

            if (miniGameContainer) miniGameContainer.SetActive(false);
            if (hideOnStart) hideOnStart.SetActive(true);

            Notify(GameEvents.KillThoughtsAndSayings);
            Notify(GameEvents.KillDialog);
            Notify(GameEvents.MiniGameClosed);


            // Get a random dialog line based on the game outcome
            NarrationDialogLine dialogLine = null;
            if (dialogLines != null)
                dialogLine = GetRandomLine(
                    isGameWon ? DialogLineType.Good : DialogLineType.Bad
                );

            // Trigger the dialog line if one was found
            if (dialogLine != null) Notify(GameEvents.TriggerSpecificDialogLine, dialogLine);

            // Notify about the game outcome
            Notify(isGameWon ? GameEvents.MiniGameWon : GameEvents.MiniGameLost, score);
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