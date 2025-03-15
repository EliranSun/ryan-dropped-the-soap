using System;
using Dialog.Scripts;
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
        private const float MinMoveSpeed = 2f; // Minimum move speed to ensure player can still move
        private const float MaxMoveSpeed = 15f; // Maximum move speed to prevent too fast movement

        [SerializeField] private GameObject player;
        [SerializeField] private MiniGameName[] instructions;
        [SerializeField] private Slider scoreSlider;
        [SerializeField] private TextMeshProUGUI inGameInstructionsText;
        [SerializeField] private GameObject inGameInstructions;
        [SerializeField] private float currentScore;
        [SerializeField] private int initiateMiniGameDelay = 3;
        [SerializeField] private readonly int bestEmployeeScore = 100;
        [SerializeField] private readonly int bossOfficeScore = -100;
        [SerializeField] private NarrationDialogLine goodEndingDialogLine;
        [SerializeField] private NarrationDialogLine badEndingDialogLine;
        private bool _isMiniGameInitiated;

        private void Start()
        {
            scoreSlider.minValue = -100;
            scoreSlider.maxValue = 100;

            // set color to half the brightness of player sprite
            var playerSprite = player.GetComponent<SpriteRenderer>();
            playerSprite.color = new Color(0.5f, 0.5f, 0.5f, 1f);

            inGameInstructions.SetActive(false);
        }

        private void SetRandomInstruction()
        {
            if (instructions == null || instructions.Length == 0)
            {
                print("Instructions array is empty or null!");
                return;
            }

            inGameInstructions.SetActive(true);

            var randomIndex = Random.Range(0, instructions.Length);
            var selectedInstruction = instructions[randomIndex];

            print($"Selected instruction: {selectedInstruction} in game instructions: {inGameInstructions}");

            if (inGameInstructions != null && inGameInstructions.activeInHierarchy)
            {
                inGameInstructionsText.text = selectedInstruction + "!";
                TriggerMiniGame(selectedInstruction);
                // StartMiniGame(); // counts down
            }
            else
                print("inGameInstructions error!");
        }

        public override void OnNotify(GameEventData eventData)
        {
            base.OnNotify(eventData);

            if (eventData.Name == GameEvents.StartMiniGames)
            {
                Invoke(nameof(SetRandomInstruction), initiateMiniGameDelay);
            }

            if (eventData.Name == GameEvents.ThoughtScoreChange)
            {
                var newScore = (int)eventData.Data;
                currentScore += newScore;
            }

            if (eventData.Name == GameEvents.MiniGameStart && !_isMiniGameInitiated)
            {
                print("MiniGameStart");
                _isMiniGameInitiated = true;
                // CloseInstruction();
                // CloseMiniGame();
                // stopping the clock, a bit confusing but this is the mini-game 
                // of starting a mini-game
            }

            // if (eventData.Name == GameEvents.MiniGameClosed)
            //     if (_isMiniGameInitiated)
            //         OnMiniGameEnd();

            if (eventData.Name == GameEvents.MiniGameWon)
            {
                print("@@@@@@@ GAME WON");
                currentScore += 10f;
                _isMiniGameInitiated = false;
                inGameInstructionsText.text = "GOOD EMPLOYEE";
                CloseMiniGame();
                Notify(GameEvents.SlowDownMusic);
                Invoke(nameof(OnMiniGameEnd), 2f);
            }

            if (eventData.Name == GameEvents.MiniGameLost)
            {
                print("@@@@@@@ GAME LOST");
                currentScore -= 10f;
                _isMiniGameInitiated = false;
                inGameInstructionsText.text = "BAD EMPLOYEE";
                CloseMiniGame();
                Notify(GameEvents.SpeedUpMusic);
                Invoke(nameof(OnMiniGameEnd), 2f);
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

        private void OnMiniGameEnd()
        {
            print("OnMiniGameEnd");
            CloseInstruction();

            var playerSprite = player.GetComponent<SpriteRenderer>();
            var playerMovement = player.GetComponent<WobblyMovement>();

            if (currentScore < scoreSlider.value)
            {
                // lower = towards boss office end scene, this makes zeke happy
                playerSprite.color = IncreaseBrightness(playerSprite.color);
                playerMovement.moveSpeed += 2f;
                // Clamp move speed to maximum value
                playerMovement.moveSpeed = Mathf.Min(playerMovement.moveSpeed, MaxMoveSpeed);
            }
            else
            {
                // higher = towards perfect employee end scene, this makes zeke depressed
                playerSprite.color = DecreaseBrightness(playerSprite.color);
                playerMovement.moveSpeed -= 2f;
                // Clamp move speed to minimum value
                playerMovement.moveSpeed = Mathf.Max(playerMovement.moveSpeed, MinMoveSpeed);
            }

            scoreSlider.value = currentScore;

            Notify(GameEvents.ResetThoughtsAndSayings);

            if (currentScore >= bestEmployeeScore || currentScore <= bossOfficeScore)
            {
                Notify(GameEvents.StopMusic);
                Notify(GameEvents.TriggerSpecificDialogLine, currentScore >= bestEmployeeScore
                ? goodEndingDialogLine
                : badEndingDialogLine);
                return;
            }

            SetRandomInstruction();
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