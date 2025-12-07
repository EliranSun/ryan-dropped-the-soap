using System;
using Dialog;
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
        Lunch,
        LockPick,
        Shout
    }

    // TODO: Strange that Mini games manager is also a mini game
    public class MiniGamesManager : MiniGame
    {
        [Header("Mini Games Manager")] private const float BrightnessDecrease = 0.1f;
        private const float BrightnessIncrease = 0.1f;
        private const float MinMoveSpeed = 2f; // Minimum move speed to ensure player can still move
        private const float MaxMoveSpeed = 15f; // Maximum move speed to prevent too fast movement
        private const int BestEmployeeScore = 100;
        private const int BossOfficeScore = -100;
        [SerializeField] private float currentScore;
        [SerializeField] private int pointsPerGame = 10;
        [SerializeField] private bool areGamesRandomized;
        [SerializeField] private GameObject player;
        [SerializeField] private MiniGameName[] instructions;
        [SerializeField] private Slider scoreSlider;
        [SerializeField] private GameObject scoreWrapper;
        [SerializeField] private TextMeshProUGUI inGameInstructionsText;
        [SerializeField] private GameObject inGameInstructions;
        [SerializeField] private int initiateMiniGameDelay = 3;
        [SerializeField] private NarrationDialogLine goodEndingDialogLine;
        [SerializeField] private NarrationDialogLine badEndingDialogLine;
        [SerializeField] private GameObject badEndingTrigger;
        [SerializeField] private GameObject goodEndingTrigger;
        private bool _isMiniGameInitiated;
        private int _miniGamesIndex;

        private void Start()
        {
            scoreSlider.minValue = -100;
            scoreSlider.maxValue = 100;
            scoreSlider.value = currentScore;

            // set color to half the brightness of player sprite
            var playerSprite = player.GetComponent<SpriteRenderer>();
            playerSprite.color = new Color(0.5f, 0.5f, 0.5f, 1f);

            if (inGameInstructions) inGameInstructions.SetActive(false);
            if (badEndingTrigger) badEndingTrigger.SetActive(false);
            if (goodEndingTrigger) goodEndingTrigger.SetActive(false);
        }

        private void SetNextInstruction()
        {
            if (instructions == null || instructions.Length == 0)
            {
                print("Instructions array is empty or null!");
                return;
            }

            inGameInstructions.SetActive(true);


            var selectedInstruction = areGamesRandomized
                ? instructions[Random.Range(0, instructions.Length)]
                : instructions[_miniGamesIndex];

            _miniGamesIndex += 1;

            if (_miniGamesIndex > instructions.Length && !areGamesRandomized)
            {
                print("MOving on");
                return;
            }

            print($"Selected instruction: {selectedInstruction} in game instructions: {inGameInstructions}");

            if (inGameInstructions != null && inGameInstructions.activeInHierarchy)
            {
                inGameInstructionsText.text = selectedInstruction + "!";
                TriggerMiniGame(selectedInstruction);
                StartMiniGame();
            }
            else
            {
                print("inGameInstructions error!");
            }
        }

        public void OnNotify(GameEventData eventData)
        {
            // base.OnNotify(eventData);

            if (eventData.Name == GameEvents.StartMiniGames)
                Invoke(nameof(SetNextInstruction), initiateMiniGameDelay);

            // if (eventData.Name == GameEvents.ThoughtScoreChange)
            // {
            //     var newScore = (int)eventData.Data;
            //     print("@@@@@@@ SCORE CHANGE: " + newScore);
            //     currentScore += newScore * 3;
            // }

            if (eventData.Name == GameEvents.MiniGameStart && !_isMiniGameInitiated)
                _isMiniGameInitiated = true;

            if (eventData.Name == GameEvents.MiniGameWon)
            {
                var gameScore = (int)eventData.Data;
                print($"GAME WON WITH {gameScore}");
                currentScore += gameScore != 0 ? gameScore : pointsPerGame;
                _isMiniGameInitiated = false;
                inGameInstructionsText.text = "GOOD EMPLOYEE";
                // CloseMiniGame();
                Notify(GameEvents.SlowDownMusic);
                Invoke(nameof(OnMiniGameEnd), 2f);
            }

            if (eventData.Name == GameEvents.MiniGameLost)
            {
                // TODO: Fixed score vs. outcome score. which is better
                var gameScore = (int)eventData.Data;
                print($"GAME LOST WITH {gameScore}");
                currentScore += gameScore != 0 ? gameScore : -pointsPerGame;
                _isMiniGameInitiated = false;
                inGameInstructionsText.text = "BAD EMPLOYEE";
                // CloseMiniGame();
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

            if (currentScore is >= BestEmployeeScore or <= BossOfficeScore)
            {
                Notify(GameEvents.StopMusic);

                var isGoodEnding = currentScore >= BestEmployeeScore;

                if (badEndingTrigger) badEndingTrigger.SetActive(!isGoodEnding);
                if (goodEndingTrigger) goodEndingTrigger.SetActive(isGoodEnding);

                Notify(GameEvents.TriggerSpecificDialogLine,
                    isGoodEnding
                        ? goodEndingDialogLine
                        : badEndingDialogLine);

                CloseInstruction();
                StopAllCoroutines();
                scoreWrapper.SetActive(false);

                return;
            }

            SetNextInstruction();
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