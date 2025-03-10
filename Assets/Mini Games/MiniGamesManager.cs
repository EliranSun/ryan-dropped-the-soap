using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Mini_Games
{
    [Serializable]
    public enum MiniGameName
    {
        Reply,
        Flirt,
        Organize,
        File,
        Lunch
    }

    public class MiniGamesManager : MiniGame
    {
        [Header("Mini Games Manager")] private const float BrightnessDecrease = 0.1f;
        private const float BrightnessIncrease = 0.1f;

        [SerializeField] private GameObject player;
        [SerializeField] private MiniGameName[] instructions;
        [SerializeField] private Slider scoreSlider;
        [SerializeField] private TextMeshProUGUI inGameInstructionsText;
        [SerializeField] private GameObject inGameInstructions;
        private bool _isMiniGameInitiated;
        private float _score = 0.5f;

        private void Start()
        {
            scoreSlider.minValue = 0;
            scoreSlider.maxValue = 1;

            // set color to half the brighness of player sprite
            var playerSprite = player.GetComponent<SpriteRenderer>();
            playerSprite.color = new Color(0.5f, 0.5f, 0.5f, 1f);

            inGameInstructions.SetActive(false);
            Invoke(nameof(SetRandomInstruction), 5f);
        }

        private void SetRandomInstruction()
        {
            if (instructions == null || instructions.Length == 0)
            {
                Debug.LogWarning("Instructions array is empty or null!");
                return;
            }

            inGameInstructions.SetActive(true);

            var randomIndex = Random.Range(0, instructions.Length);
            var selectedInstruction = instructions[randomIndex];

            if (inGameInstructions != null)
            {
                StartMiniGame(); // counts down
                inGameInstructionsText.text = selectedInstruction + "!";
                TriggerMiniGame(selectedInstruction);
                // Invoke(nameof(CloseInstruction), 5f);
            }
            else
            {
                Debug.LogWarning("inGameInstructions TextMeshProUGUI reference is null!");
            }
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.MiniGameStart && !_isMiniGameInitiated)
            {
                _isMiniGameInitiated = true;
                CloseInstruction();
                // stopping the clock, a bit confusing but this is the mini-game 
                // of starting a mini-game
                CloseMiniGame();
            }

            if (eventData.Name == GameEvents.MiniGameClosed && !_isMiniGameInitiated)
                Invoke(nameof(SetRandomInstruction), 2f);

            if (!_isMiniGameInitiated)
                return;

            if (eventData.Name == GameEvents.MiniGameWon)
            {
                OnMiniGameWin();
                Invoke(nameof(SetRandomInstruction), 2f);
            }

            if (eventData.Name == GameEvents.MiniGameClosed)
            {
                OnMiniGameLose();
                Invoke(nameof(SetRandomInstruction), 2f);
            }
        }

        private void CloseInstruction()
        {
            inGameInstructions.SetActive(false);
        }

        private void TriggerMiniGame(MiniGameName miniGameNameName)
        {
            Notify(GameEvents.MiniGameIndicationTrigger, miniGameNameName);
        }

        private void OnMiniGameWin()
        {
            CloseInstruction();

            _score += 0.1f;
            scoreSlider.value = _score;
            var playerSprite = player.GetComponent<SpriteRenderer>();
            var playerMovement = player.GetComponent<WobblyMovement>();

            playerSprite.color = DecreaseBrightness(playerSprite.color);
            playerMovement.moveSpeed -= 2f;
        }

        private void OnMiniGameLose()
        {
            CloseInstruction();

            _score -= 0.1f;
            scoreSlider.value = _score;

            var playerSprite = player.GetComponent<SpriteRenderer>();
            playerSprite.color = IncreaseBrightness(playerSprite.color);

            var playerMovement = player.GetComponent<WobblyMovement>();
            playerMovement.moveSpeed += 2f;
        }

        private static Color DecreaseBrightness(Color currentColor)
        {
            return new Color(
                Mathf.Max(currentColor.r - BrightnessDecrease, 0f),
                Mathf.Max(currentColor.g - BrightnessDecrease, 0f),
                Mathf.Max(currentColor.b - BrightnessDecrease, 0f),
                currentColor.a
            );
        }

        private static Color IncreaseBrightness(Color currentColor)
        {
            return new Color(
                Mathf.Min(currentColor.r + BrightnessIncrease, 1f),
                Mathf.Min(currentColor.g + BrightnessIncrease, 1f),
                Mathf.Min(currentColor.b + BrightnessIncrease, 1f),
                currentColor.a
            );
        }
    }
}