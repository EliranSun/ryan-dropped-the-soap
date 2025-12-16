using System;
using System.Collections;
using Dialog;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Mini_Games
{
    [Serializable]
    public enum MiniGameName
    {
        Neglect,
        Attend,
        Reply,
        Flirt,
        Organize,
        File,
        Lunch,
        LockPick,
        Shout,
        Snooze,
        SurpassFeelings,
        DetectFeelings,
        Avoid
    }

    public enum PlayStyle
    {
        ScoreBased,
        InstructionBased
    }

    // TODO: Strange that Mini games manager is also a mini game
    public class MiniGamesManager : ObserverSubject
    {
        private const int GamesWinScore = 100;
        private const int GamesLoseScore = -100;


        [Header("Side Effects - should move to another class")]
        private const float BrightnessDecrease = 0.1f;

        private const float BrightnessIncrease = 0.1f;
        private const float MinMoveSpeed = 5f; // Minimum move speed to ensure player can still move
        private const float MaxMoveSpeed = 15f; // Maximum move speed to prevent too fast movement

        [Header("Settings")] [SerializeField] private bool areGamesRandomized;

        [SerializeField] private int defaultScorePerGame = 10;
        [SerializeField] private int initiateMiniGameDelay = 3;
        [SerializeField] private int loopThroughInstructionsCount;
        [SerializeField] private PlayStyle playStyle;
        [SerializeField] private TextMeshProUGUI inGameInstructionsText;
        [SerializeField] private string winGameText;
        [SerializeField] private string loseGameText;
        [SerializeField] private GameEvents endEvent;
        [SerializeField] private GameObject[] lives;
        [SerializeField] private MiniGameName[] instructions;
        [SerializeField] private Slider scoreSlider;
        [SerializeField] private GameObject allGamesContainer;
        [SerializeField] private GameObject scoreWrapper;
        [SerializeField] private GameObject inGameInstructions;
        [SerializeField] private GameObject player;
        [SerializeField] private NarrationDialogLine endingDialogLine;
        [SerializeField] private NarrationDialogLine goodEndingDialogLine;
        [SerializeField] private NarrationDialogLine badEndingDialogLine;
        [SerializeField] private NarrationDialogLine miniGameWonDialogLine;
        [SerializeField] private NarrationDialogLine miniGameLostDialogLine;
        [SerializeField] private GameObject badEndingTrigger;
        [SerializeField] private GameObject goodEndingTrigger;
        private bool _isMiniGameInitiated;
        private int _livesCount;
        private bool _miniGamesEnded;
        private int _miniGamesIndex;
        private int _score;

        private MiniGameName _selectedInstruction;

        private void Start()
        {
            scoreSlider.minValue = -100;
            scoreSlider.maxValue = 100;
            scoreSlider.value = _score;

            // set color to half the brightness of player sprite
            var playerSprite = player.GetComponent<SpriteRenderer>();
            playerSprite.color = new Color(0.5f, 0.5f, 0.5f, 1f);

            if (inGameInstructions) inGameInstructions.SetActive(false);
            if (badEndingTrigger) badEndingTrigger.SetActive(false);
            if (goodEndingTrigger) goodEndingTrigger.SetActive(false);

            _livesCount = lives.Length;
        }

        private void SetNextInstruction()
        {
            if (instructions == null || instructions.Length == 0)
            {
                print("Instructions array is empty or null!");
                return;
            }

            if (_miniGamesIndex > instructions.Length && !areGamesRandomized)
            {
                print("MOving on");
                return;
            }

            _selectedInstruction = GetNextMiniGameName();
            Notify(GameEvents.MiniGameStart, _selectedInstruction);

            inGameInstructions.SetActive(true);

            print($"Selected instruction: {_selectedInstruction} in game instructions: {inGameInstructions}");

            if (inGameInstructions != null && inGameInstructions.activeInHierarchy)
            {
                inGameInstructionsText.text = _selectedInstruction + "!";
                StartCoroutine(TriggerMiniGameDelayed(_selectedInstruction));
            }
            else
            {
                print("inGameInstructions error!");
            }
        }

        private MiniGameName GetNextMiniGameName()
        {
            return areGamesRandomized
                ? instructions[Random.Range(0, instructions.Length)]
                : instructions[_miniGamesIndex];
        }

        public void OnNotify(GameEventData eventData)
        {
            if (_miniGamesEnded) return;

            switch (eventData.Name)
            {
                case GameEvents.StartMiniGames:
                    Invoke(nameof(SetNextInstruction), initiateMiniGameDelay);
                    break;

                case GameEvents.EndMiniGames:
                    EndMiniGames();
                    break;

                case GameEvents.MiniGameStart when !_isMiniGameInitiated:
                    _isMiniGameInitiated = true;
                    break;

                case GameEvents.MiniGameWon:
                {
                    print("GAME WON");
                    var gameScore = (int)eventData.Data;

                    _score += gameScore != 0 ? gameScore : defaultScorePerGame;

                    _isMiniGameInitiated = false;
                    inGameInstructionsText.text = winGameText;
                    StartCoroutine(MiniGameEndDialog(miniGameWonDialogLine));
                    Notify(GameEvents.SlowDownMusic);
                    Invoke(nameof(OnMiniGameEnd), 2f);
                    break;
                }

                case GameEvents.MiniGameLost:
                {
                    print("GAME LOST");
                    LoseLife();

                    // TODO: Fixed score vs. outcome score. which is better
                    var gameScore = (int)eventData.Data;
                    _score += gameScore != 0 ? gameScore : -defaultScorePerGame;
                    _isMiniGameInitiated = false;
                    inGameInstructionsText.text = loseGameText;
                    StartCoroutine(MiniGameEndDialog(miniGameLostDialogLine));
                    Notify(GameEvents.SpeedUpMusic);
                    Invoke(nameof(OnMiniGameEnd), 2f);
                    break;
                }
            }
        }

        private IEnumerator MiniGameEndDialog(NarrationDialogLine line)
        {
            yield return new WaitForSeconds(2);
            Notify(GameEvents.TriggerSpecificDialogLine, line);
        }

        private void LoseLife()
        {
            if (lives.Length == 0)
                return;

            _livesCount--;

            if (_livesCount <= 0)
                // restart scene
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            // Fix: Only set the last remaining active life to false when losing a life
            for (var i = lives.Length - 1; i >= 0; i--)
                if (lives[i].activeSelf)
                {
                    lives[i].SetActive(false);
                    break;
                }
        }

        private void CloseInstruction()
        {
            inGameInstructions.SetActive(false);
        }

        private IEnumerator TriggerMiniGameDelayed(MiniGameName miniGameNameName)
        {
            yield return new WaitForSeconds(2f);
            inGameInstructions.SetActive(false);
            TriggerMiniGame(miniGameNameName);
        }

        private void TriggerMiniGame(MiniGameName miniGameNameName)
        {
            Notify(GameEvents.MiniGameIndicationTrigger, miniGameNameName);
        }

        private void OnMiniGameEnd()
        {
            Notify(GameEvents.MiniGameEnded, instructions[_miniGamesIndex]);

            _miniGamesIndex += 1;

            print("OnMiniGameEnd");
            CloseInstruction();

            var playerSprite = player.GetComponent<SpriteRenderer>();
            var playerMovement = player.GetComponent<WobblyMovement>();

            if (_score < scoreSlider.value)
            {
                // lower = towards boss office end scene, this makes zeke happy
                playerSprite.color = IncreaseBrightness(playerSprite.color);
                playerMovement.moveSpeed += 1f;
                // Clamp move speed to maximum value
                playerMovement.moveSpeed = Mathf.Min(playerMovement.moveSpeed, MaxMoveSpeed);
            }
            else
            {
                // higher = towards perfect employee end scene, this makes zeke depressed
                playerSprite.color = DecreaseBrightness(playerSprite.color);
                playerMovement.moveSpeed -= 1f;
                // Clamp move speed to minimum value
                playerMovement.moveSpeed = Mathf.Max(playerMovement.moveSpeed, MinMoveSpeed);
            }

            scoreSlider.value = _score;

            Notify(GameEvents.ResetThoughtsAndSayings);

            if (playStyle == PlayStyle.InstructionBased && _miniGamesIndex > instructions.Length - 1)
                _miniGamesIndex = 0;

            if (playStyle == PlayStyle.ScoreBased && _score is >= GamesWinScore or <= GamesLoseScore)
            {
                EndMiniGames();
                return;
            }

            SetNextInstruction();
        }

        private void EndMiniGames()
        {
            _miniGamesEnded = true;
            Notify(GameEvents.StopMusic);

            // var isGoodEnding = _score >= GamesWinScore;
            //
            // if (badEndingTrigger) badEndingTrigger.SetActive(!isGoodEnding);
            // if (goodEndingTrigger) goodEndingTrigger.SetActive(isGoodEnding);
            //

            Notify(endEvent);
            Notify(GameEvents.TriggerSpecificDialogLine, endingDialogLine);

            CloseInstruction();
            StopAllCoroutines();

            scoreWrapper.SetActive(false);
            allGamesContainer.SetActive(false);
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