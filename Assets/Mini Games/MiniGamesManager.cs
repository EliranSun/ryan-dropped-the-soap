using System;
using System.Collections;
using Dialog;
using Mini_Games.Organize_Desk.scripts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    // Two ways to deal with the clock & mini-games:

    // 1.   -> Clock -> Time -> Mini game

    // 2.   -> Clock -> Time
    //      -> Mini game order

    // TODO: Strange that Mini games manager is also a mini game
    public class MiniGamesManager : ObserverSubject
    {
        [Header("Side Effects - should move to another class")]
        private const float BrightnessDecrease = 0.1f;

        private const float BrightnessIncrease = 0.1f;
        private const float MinMoveSpeed = 5f; // Minimum move speed to ensure player can still move
        private const float MaxMoveSpeed = 15f; // Maximum move speed to prevent too fast movement

        [Header("Settings")] [SerializeField] private bool areGamesRandomized;

        [SerializeField] private int initiateMiniGameDelay = 3;
        [SerializeField] private int loopThroughInstructionsCount;
        [SerializeField] private PlayStyle playStyle;
        [SerializeField] private TextMeshProUGUI inGameInstructionsText;
        [SerializeField] private string winGameText;
        [SerializeField] private string loseGameText;
        [SerializeField] private GameEvents endEvent;
        [SerializeField] private GameObject[] lives;
        [SerializeField] private MiniGame[] miniGames;
        [SerializeField] private MiniGameName[] instructions;
        [SerializeField] private GameObject allGamesContainer;
        [SerializeField] private GameObject inGameInstructions;
        [SerializeField] private GameObject player;
        [SerializeField] private NarrationDialogLine endingDialogLine;
        [SerializeField] private NarrationDialogLine goodEndingDialogLine;
        [SerializeField] private NarrationDialogLine badEndingDialogLine;
        [SerializeField] private NarrationDialogLine miniGameWonDialogLine;
        [SerializeField] private NarrationDialogLine miniGameLostDialogLine;

        [Header("Sound")] [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip soundtrack;
        [SerializeField] private AudioClip winSound;
        [SerializeField] private AudioClip loseSound;

        [SerializeField] public TypedDialogLine[] dialogLines;

        private bool _isMiniGameInitiated;
        private int _livesCount;
        private bool _miniGamesEnded;
        private int _miniGamesIndex;
        private MiniGameName _selectedInstruction;

        private void Start()
        {
            // set color to half the brightness of player sprite
            var playerSprite = player.GetComponent<SpriteRenderer>();
            playerSprite.color = new Color(0.5f, 0.5f, 0.5f, 1f);

            if (inGameInstructions) inGameInstructions.SetActive(false);

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

        private void OnMiniGameEnd(bool isWin)
        {
            Notify(GameEvents.MiniGameEnded, instructions[_miniGamesIndex]);

            _miniGamesIndex += 1;

            CloseInstruction();

            if (isWin) LighterPlayer();
            else HeavierPlayer();

            Notify(GameEvents.ResetThoughtsAndSayings);

            if (playStyle == PlayStyle.InstructionBased && _miniGamesIndex > instructions.Length - 1)
                _miniGamesIndex = 0;

            SetNextInstruction();
        }

        private void LighterPlayer()
        {
            var playerSprite = player.GetComponent<SpriteRenderer>();
            var playerMovement = player.GetComponent<WobblyMovement>();
            // lower = towards boss office end scene, this makes zeke happy
            playerSprite.color = IncreaseBrightness(playerSprite.color);
            playerMovement.moveSpeed += 1f;
            // Clamp move speed to maximum value
            playerMovement.moveSpeed = Mathf.Min(playerMovement.moveSpeed, MaxMoveSpeed);
        }

        private void HeavierPlayer()
        {
            var playerSprite = player.GetComponent<SpriteRenderer>();
            var playerMovement = player.GetComponent<WobblyMovement>();
            // higher = towards perfect employee end scene, this makes zeke depressed
            playerSprite.color = DecreaseBrightness(playerSprite.color);
            playerMovement.moveSpeed -= 1f;
            // Clamp move speed to minimum value
            playerMovement.moveSpeed = Mathf.Max(playerMovement.moveSpeed, MinMoveSpeed);
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
    }
}