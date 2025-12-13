using System;
using System.Collections;
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
        private MiniGameName miniGameName;

        [SerializeField] private TextMeshProUGUI timerTextContainer;
        [SerializeField] private int timer = 60;
        [SerializeField] public GameObject mainCamera;
        [SerializeField] public GameObject miniGameContainer;
        [SerializeField] private GameObject hideOnStart;
        [SerializeField] public GameObject inGameTrigger;
        [SerializeField] public GameObject winState;
        [SerializeField] public GameObject loseState;

        [Header("Sound")] [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip soundtrack;
        [SerializeField] private AudioClip winSound;
        [SerializeField] private AudioClip loseSound;

        [Header("Animation Settings")] [SerializeField]
        private float animationDuration = 1f;

        [SerializeField] private float jumpHeight = 0.5f;
        [SerializeField] private float shakeIntensity = 0.2f;

        [Header("Game dialog responses")] [SerializeField]
        public TypedDialogLine[] dialogLines;

        public int score;
        public bool isGameActive;

        private float _currentTime;
        private bool _isTimerRunning;

        protected virtual void Update()
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

        private void UpdateTimer()
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
            if (miniGameContainer) miniGameContainer.SetActive(true);
            // var newPosition = mainCamera.transform.position;
            // newPosition.z = miniGameContainer.transform.position.z;
            // miniGameContainer.transform.position = newPosition;

            if (audioSource)
            {
                audioSource.clip = soundtrack;
                audioSource.Play();
            }
        }

        protected virtual void CloseMiniGame(bool isGameWon = false)
        {
            _isTimerRunning = false;
            isGameActive = false;

            if (miniGameContainer) miniGameContainer.SetActive(false);

            switch (isGameWon)
            {
                case true when winState:
                    winState.SetActive(true);
                    StartCoroutine(PlayWinAnimation(winState));
                    break;
                case false when loseState:
                    loseState.SetActive(true);
                    StartCoroutine(PlayLoseAnimation(loseState));
                    break;
            }

            var dialogLine = GetRandomLine(isGameWon ? DialogLineType.Good : DialogLineType.Bad);
            if (dialogLine) Notify(GameEvents.TriggerSpecificDialogLine, dialogLine);
            Notify(isGameWon ? GameEvents.MiniGameWon : GameEvents.MiniGameLost, score);

            if (audioSource)
            {
                audioSource.Stop();
                audioSource.PlayOneShot(isGameWon ? winSound : loseSound);
            }

            Invoke(nameof(MiniGameCleanups), 4);
        }

        private void MiniGameCleanups()
        {
            if (winState) winState.SetActive(false);
            if (loseState) loseState.SetActive(false);
            if (hideOnStart) hideOnStart.SetActive(true);

            Notify(GameEvents.KillThoughtsAndSayings);
            Notify(GameEvents.KillDialog);
            Notify(GameEvents.MiniGameClosed);
        }

        private IEnumerator PlayWinAnimation(GameObject target)
        {
            if (target == null) yield break;

            yield return new WaitForSeconds(1);

            var targetTransform = target.transform;
            var originalPosition = targetTransform.position;
            var originalScale = targetTransform.localScale;

            // Happy celebration sequence: flip, jump, scale bounce
            var elapsedTime = 0f;

            // Phase 1: Quick scale up and flip
            while (elapsedTime < animationDuration * 0.3f)
            {
                var progress = elapsedTime / (animationDuration * 0.3f);
                var scaleMultiplier = 1f + Mathf.Sin(progress * Mathf.PI) * 0.3f;
                targetTransform.localScale = originalScale * scaleMultiplier;

                // Flip effect by scaling X negative and back
                if (progress < 0.5f)
                    targetTransform.localScale = new Vector3(-originalScale.x * scaleMultiplier,
                        originalScale.y * scaleMultiplier,
                        originalScale.z * scaleMultiplier);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Reset scale
            targetTransform.localScale = originalScale;

            // Phase 2: Jump animation
            elapsedTime = 0f;
            while (elapsedTime < animationDuration * 0.7f)
            {
                var progress = elapsedTime / (animationDuration * 0.7f);
                var jumpOffset = Mathf.Sin(progress * Mathf.PI * 2) * jumpHeight;
                var scaleOffset = 1f + Mathf.Sin(progress * Mathf.PI * 4) * 0.1f;

                targetTransform.position = originalPosition + Vector3.up * jumpOffset;
                targetTransform.localScale = originalScale * scaleOffset;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Reset to original state
            targetTransform.position = originalPosition;
            targetTransform.localScale = originalScale;
        }

        private IEnumerator PlayLoseAnimation(GameObject target)
        {
            if (target == null) yield break;

            yield return new WaitForSeconds(1);

            var targetTransform = target.transform;
            var originalPosition = targetTransform.position;
            var originalScale = targetTransform.localScale;

            // Sad defeat sequence: shake, shrink, droop
            var elapsedTime = 0f;

            // Phase 1: Shake and shrink
            while (elapsedTime < animationDuration * 0.5f)
            {
                var progress = elapsedTime / (animationDuration * 0.5f);

                // Shake effect - random offset
                var shakeOffset = new Vector3(
                    Random.Range(-shakeIntensity, shakeIntensity),
                    Random.Range(-shakeIntensity * 0.5f, shakeIntensity * 0.5f),
                    0f
                );

                // Shrinking scale
                var shrinkAmount = 1f - progress * 0.2f;
                targetTransform.localScale = originalScale * shrinkAmount;
                targetTransform.position = originalPosition + shakeOffset;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Phase 2: Droop down (sad slump)
            elapsedTime = 0f;
            while (elapsedTime < animationDuration * 0.5f)
            {
                var progress = elapsedTime / (animationDuration * 0.5f);

                // Droop effect - scale Y down and position slightly down
                var droopScale = 1f - progress * 0.3f;
                var droopOffset = -progress * 0.1f;

                targetTransform.localScale = new Vector3(
                    originalScale.x * 0.8f,
                    originalScale.y * droopScale,
                    originalScale.z * 0.8f
                );
                targetTransform.position = originalPosition + Vector3.up * droopOffset;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Hold the sad pose briefly
            yield return new WaitForSeconds(0.3f);

            // Slowly return to original state
            elapsedTime = 0f;
            var currentScale = targetTransform.localScale;
            var currentPos = targetTransform.position;

            while (elapsedTime < 0.5f)
            {
                var progress = elapsedTime / 0.5f;

                targetTransform.localScale = Vector3.Lerp(currentScale, originalScale, progress);
                targetTransform.position = Vector3.Lerp(currentPos, originalPosition, progress);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure exact reset
            targetTransform.position = originalPosition;
            targetTransform.localScale = originalScale;
        }

        public virtual void OnNotify(GameEventData eventData)
        {
            switch (eventData.Name)
            {
                case GameEvents.StartMiniGames:
                    var eventMiniGameName = (MiniGameName)eventData.Data;
                    if (eventMiniGameName == miniGameName) StartMiniGame();
                    break;

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