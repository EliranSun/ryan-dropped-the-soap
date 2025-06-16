using System;
using System.Collections;
using System.Linq;
using Dialog;
using Dialog.Scripts;
using museum_dialog.scripts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Character_Creator.scripts
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(DialogueStateChanger))]
    public class DialogueManager : ObserverSubject
    {
        [SerializeField] private GameObject playerChoiceButton;
        [SerializeField] private GameObject playerChoiceButtonsContainer;
        [SerializeField] private GameObject playerInconsequentialChoice;
        [SerializeField] private GameObject playerInputsContainer;
        [SerializeField] private GameObject playerTextInput;
        [SerializeField] private TextMeshProUGUI narratorText;
        [SerializeField] private EventToDialogMap eventToDialogMap;
        [SerializeField] private AudioSource soundEffectsAudioSource;
        [SerializeField] private EventToSound eventToSound;
        [SerializeField] private SpriteRenderer overlayImage;
        [SerializeField] private int delayFirstLine;

        private bool _asyncLinesReady;
        private AudioSource _audioSource;
        private NarrationDialogLine _currentDialogue;
        private PlayerData _player;
        private InteractableObjectName _potentialSelectedInteractableObject;
        private InteractableObjectType _potentialSelectedInteractableObjectType;
        private NarrationDialogLine _triggeredDialogueLine;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            Invoke(nameof(Init), delayFirstLine);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) FastForwardDialog();
            if (Input.GetKeyUp(KeyCode.Space)) NormalSpeedDialog();

            AdvanceDialogOnClick();
        }

        private void Init()
        {
            var nextLine = DialogueStateChanger.Instance.GetDialogStateByPlayerPrefs();
            if (!nextLine) return;

            UpdateDialogState(nextLine);
            StartCoroutine(HandlePlayerNameLinesAndStartRead());
        }

        private IEnumerator HandlePlayerNameLinesAndStartRead()
        {
            var playerName = PlayerData.GetPlayerName();
            var playerGender = PlayerData.GetPlayerGender();

            // playerGender != CharacterType.None
            if (playerName != "" && !PrefetchDialogLines.Instance.isFetched)
                yield return StartCoroutine(PrefetchDialogLines.Instance.FetchAndPopulatePlayerLines());

            Invoke(nameof(ReadCurrentLine), _currentDialogue.waitBeforeLine);
        }

        private IEnumerator HandlePlayerNameLines(Action onComplete)
        {
            var playerName = PlayerData.GetPlayerName();
            var playerGender = PlayerData.GetPlayerGender();

            print(
                $"Checking if player name is empty {playerName != ""} and if dialog lines are fetched {PrefetchDialogLines.Instance.isFetched}");
            if (playerName != "" && !PrefetchDialogLines.Instance.isFetched)
            {
                print($"fetching dialog lines for player name {playerName}");
                yield return StartCoroutine(PrefetchDialogLines.Instance.FetchAndPopulatePlayerLines());
            }

            print($"invoking onComplete for player name {playerName}");
            onComplete?.Invoke();
        }

        private void HandlePlayerNameLines()
        {
            var playerName = PlayerData.GetPlayerName();
            var playerGender = PlayerData.GetPlayerGender();

            // playerGender != CharacterType.None
            if (playerName != "" && !PrefetchDialogLines.Instance.isFetched)
                StartCoroutine(PrefetchDialogLines.Instance.FetchAndPopulatePlayerLines());
        }

        // TODO: Remove these two after current method of switching lines proves good enough
        public static bool IsReversePitch(AudioSource source)
        {
            return source.pitch < 0f;
        }

        private IEnumerator CheckAudioEnd()
        {
            yield return new WaitWhile(() => _audioSource.isPlaying);

            Invoke(nameof(OnDialogEnd), _currentDialogue.wait);
        }

        private void ReadCurrentLine()
        {
            if (!_currentDialogue)
                return;

            print($"Reading current line for {_currentDialogue}");

            if (_currentDialogue.actionBeforeLine != GameEvents.None)
                TriggerAnAct(_currentDialogue.actionBeforeLine);

            var line = GetLineByGender(_currentDialogue);

            if (line == null)
            {
                OnDialogEnd();
                return;
            }

            narratorText.text = line.text
                .Replace("{playerName}", PlayerData.GetPlayerName())
                .Replace("{partnerName}", PlayerData.GetPartnerName());

            if (_currentDialogue.overlayImageSprite)
            {
                print("Fading image");
                overlayImage.sprite = _currentDialogue.overlayImageSprite;
                StartCoroutine(FadeOverlayImage());
            }

            Notify(GameEvents.ActorReaction, _currentDialogue);

            if (line.clip)
            {
                _audioSource.clip = line.clip;
                _audioSource.Play();
                Notify(GameEvents.LineNarrationStart, _currentDialogue);
                StartCoroutine(CheckAudioEnd());
            }
            else
            {
                var wordPerSecond = 2;
                var duration = narratorText.text.Split(' ').Length / wordPerSecond;
                Invoke(nameof(OnDialogEnd), _currentDialogue.wait + duration);
            }
        }

        private static VoicedLine GetLineByGender(NarrationDialogLine dialogueLineObject)
        {
            try
            {
                var gender = PlayerData.GetPlayerGender();
                return dialogueLineObject.voicedLines.First(voicedLine =>
                {
                    if (voicedLine.gender is CharacterType.NonBinary or CharacterType.None)
                        return true;

                    return voicedLine.gender == gender;
                });
            }
            catch (ArgumentException error)
            {
                print(error);
            }

            return null;
        }

        private void OnDialogEnd()
        {
            HandlePlayerNameLines();

            if (_currentDialogue.toggleLineCondition)
                _currentDialogue.toggleLineCondition.lineCondition.isMet = true;

            PostDialogAdvanceActions();
        }

        private void PostDialogAdvanceActions()
        {
            // TODO: Add a specific action for removing overlay images
            // if (_currentDialogue.overlayImageSprite && overlayImage)
            // {
            //     overlayImage.color = Color.clear;
            //     overlayImage.sprite = null;
            // }

            if (!_currentDialogue)
                return;

            if (_currentDialogue.playerOptions?.Length > 0 && NoPlayerInputsExist())
                // GeneratePlayerInputs(_currentDialogue);
                if (_currentDialogue.mandatoryPlayerChoice)
                    return;

            if (_currentDialogue.actionAfterLine != GameEvents.None)
                TriggerAnAct(_currentDialogue.actionAfterLine);

            if (_currentDialogue.playerReactions?.Length > 0)
                return;

            if (_currentDialogue.randomizedDialogLines?.Length > 0)
            {
                var randomIndex = Random.Range(0, _currentDialogue.randomizedDialogLines.Length);
                UpdateDialogState(_currentDialogue.randomizedDialogLines[randomIndex]);
                ReadCurrentLine();
                return;
            }

            if (_currentDialogue.objectReferringDialogLines?.Length > 0)
            {
                var choiceType = _currentDialogue.objectReferringDialogLines[0].objectType;
                var playerChosenObject = PlayerPrefs.GetString(choiceType.ToString());


                if (playerChosenObject != "")
                {
                    var dialogLine = _currentDialogue.objectReferringDialogLines
                        .First(obj => obj.objectName.ToString() == playerChosenObject);

                    if (dialogLine.line)
                    {
                        UpdateDialogState(dialogLine.line);
                        ReadCurrentLine();
                    }
                }

                return;
            }

            if (_triggeredDialogueLine != null)
            {
                UpdateDialogState(_triggeredDialogueLine);
                _triggeredDialogueLine = null;
                ReadCurrentLine();
            }
        }

        private bool NoPlayerInputsExist()
        {
            var objects = GameObject.FindGameObjectsWithTag("PlayerInput");
            return objects.Length == 0;
        }

        private void TriggerAnAct(GameEvents action)
        {
            switch (action)
            {
                case GameEvents.NextScene:
                    Invoke(nameof(LoadNextScene), 4f);
                    break;

                case GameEvents.None:
                    break;

                case GameEvents.ClearDialogImageOverlay:
                    StartCoroutine(FadeOverlayImage());
                    break;

                default:
                    Notify(action);
                    break;
            }
        }

        private void LoadNextScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void OnPlayerChoiceButtonClick(NarrationDialogLine nextLine)
        {
            foreach (var input in GameObject.FindGameObjectsWithTag("PlayerInput"))
                Destroy(input);

            UpdateDialogState(nextLine);

            if (nextLine.voicedLines.First().text.Contains("{playerName}"))
            {
                print("Line contains {playerName}, starting coroutine");
                StartCoroutine(HandlePlayerNameLines(ReadCurrentLine));
            }
            else
            {
                HandlePlayerNameLines();
                ReadCurrentLine();
            }
        }

        public void OnNotify(GameEventData gameEventData)
        {
            print("DialogueManager: OnNotify " + gameEventData.Name);

            try
            {
                var eventLine = eventToDialogMap.dialogLineEvents.First(dialogLineEvent =>
                    dialogLineEvent.eventName == gameEventData.Name
                );

                if (eventLine.dialogLine)
                {
                    TriggerLine(eventLine.dialogLine);
                    return;
                }
            }
            // ignored, this is fine and expected
            catch (InvalidOperationException)
            {
            }
            catch (NullReferenceException)
            {
            }

            switch (gameEventData.Name)
            {
                case GameEvents.TriggerSpecificDialogLine:
                case GameEvents.EnteredMuseum:
                    TriggerLine((NarrationDialogLine)gameEventData.Data);
                    break;

                case GameEvents.PlayerClickOnChoice:
                    var choiceData = (EnrichedPlayerChoice)gameEventData.Data;
                    if (choiceData.PlayerDataType == PlayerDataEnum.Name)
                        try
                        {
                            // The state is already saved, just trigger the appropriate event
                            // var interactionType = choiceData.OriginalInteraction.InteractableObjectType;
                            // var eventName = GetEventNameForInteractionType(interactionType);
                            Notify(GameEvents.PlayerEnrichedChoice, new object[]
                            {
                                choiceData.Choice,
                                choiceData.PlayerDataType
                            });
                        }
                        catch (NullReferenceException error)
                        {
                            print(error);
                        }

                    OnPlayerChoiceButtonClick(choiceData.OriginalInteraction.DialogLine);
                    break;

                case GameEvents.ClickOnNpc:
                case GameEvents.DoorClicked:
                case GameEvents.ArmchairClicked:
                case GameEvents.VaseClicked:
                case GameEvents.MirrorClicked:
                case GameEvents.PaintingClicked:
                case GameEvents.ShapeClicked:
                case GameEvents.DependentClicked:
                    var interactionData = (InteractionData)gameEventData.Data;
                    InteractionStateService.Instance.SetCurrentInteraction(interactionData);
                    TriggerLine(interactionData.DialogLine, false);
                    break;

                case GameEvents.KillDialog:
                    KillDialog();
                    break;
            }
        }

        private void TriggerLine(NarrationDialogLine line, bool overrideAudio = true)
        {
            if (!overrideAudio && _audioSource.isPlaying)
                return;

            UpdateDialogState(line);

            if (line.waitBeforeLine > 0)
                Invoke(nameof(ReadCurrentLine), line.waitBeforeLine);
            else
                ReadCurrentLine();
        }

        private void FastForwardDialog()
        {
            _audioSource.pitch = 3;
        }

        public void NormalSpeedDialog()
        {
            _audioSource.pitch = 1;
        }

        public void UpdateDialogState(NarrationDialogLine nextLineObject)
        {
            _currentDialogue = nextLineObject;
        }

        private IEnumerator FadeOverlayImage()
        {
            var fadeIn = overlayImage.color.a == 0;

            var startAlpha = fadeIn ? 0f : 1f;
            var endAlpha = fadeIn ? 1f : 0f;
            var elapsedTime = 0f;
            var transitionDuration = 1f;

            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                var alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / transitionDuration);
                overlayImage.color = new Color(1f, 1f, 1f, alpha);
                yield return null;
            }

            // Ensure the final alpha is set exactly
            overlayImage.color = new Color(1f, 1f, 1f, endAlpha);

            yield return new WaitForSeconds(1);
        }

        private void KillDialog()
        {
            if (_audioSource.isPlaying)
            {
                _audioSource.pitch = 1;
                _audioSource.Stop();
                StopAllCoroutines();
            }
        }

        // TODO: Controller support
        private void AdvanceDialogOnClick()
        {
            if (!Input.GetKeyDown(KeyCode.N))
                return;

            narratorText.text = "";

            PostDialogAdvanceActions();

            Notify(GameEvents.LineNarrationEnd, new
            {
                _currentDialogue.actorName,
                _currentDialogue.playerOptions,
                _currentDialogue
            });

            if (_currentDialogue.nextDialogueLine)
            {
                UpdateDialogState(_currentDialogue.nextDialogueLine);
                ReadCurrentLine();
                return;
            }

            PostDialogAdvanceActions();
        }
    }
}