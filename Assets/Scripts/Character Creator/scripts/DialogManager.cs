using System;
using System.Collections;
using System.Linq;
using common.scripts;
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

        private bool _asyncLinesReady;
        private AudioSource _audioSource;
        private NarrationDialogLine _currentDialogue;
        private PlayerData _player;
        private InteractableObjectName _potentialSelectedInteractableObject;
        private InteractableObjectType _potentialSelectedInteractableObjectType;
        private NarrationDialogLine _triggeredDialogueLine;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            UpdateDialogState(DialogueStateChanger.Instance.GetDialogStateByPlayerPrefs());
            StartCoroutine(HandlePlayerNameLinesAndStartRead());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) OnFastForwardDialogHold();
            if (Input.GetKeyUp(KeyCode.Space)) OnFastForwardDialogRelease();
        }

        private IEnumerator HandlePlayerNameLinesAndStartRead()
        {
            var playerName = PlayerData.GetPlayerName();
            var playerGender = PlayerData.GetPlayerGender();

            if (playerName != "" && playerGender != CharacterType.None && !PrefetchDialogLines.Instance.isFetched)
                yield return StartCoroutine(PrefetchDialogLines.Instance.FetchAndPopulatePlayerLines());

            if (_currentDialogue.waitBeforeLine > 0)
                Invoke(nameof(ReadCurrentLine), _currentDialogue.waitBeforeLine);
            else
                ReadCurrentLine();
        }

        private void HandlePlayerNameLines()
        {
            var playerName = PlayerData.GetPlayerName();
            var playerGender = PlayerData.GetPlayerGender();

            if (playerName != "" && playerGender != CharacterType.None && !PrefetchDialogLines.Instance.isFetched)
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

            if (line.clip)
            {
                _audioSource.clip = line.clip;
                _audioSource.Play();
                Notify(GameEvents.LineNarrationStart, _currentDialogue.actorName);
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
            Notify(GameEvents.LineNarrationEnd, new
            {
                _currentDialogue.actorName,
                _currentDialogue.playerOptions
            });

            narratorText.text = "";
            HandlePlayerNameLines();

            // if (_currentDialogue.overlayImageSprite && overlayImage)
            // {
            //     overlayImage.color = Color.clear;
            //     overlayImage.sprite = null;
            // }

            if (_currentDialogue.playerOptions.Length > 0 && NoPlayerInputsExist())
                // GeneratePlayerInputs(_currentDialogue);
                if (_currentDialogue.mandatoryPlayerChoice)
                    return;

            if (_currentDialogue.actionAfterLine != GameEvents.None)
                TriggerAnAct(_currentDialogue.actionAfterLine);

            if (_currentDialogue.playerReactions.Length > 0)
                return;

            if (_currentDialogue.nextDialogueLine)
            {
                UpdateDialogState(_currentDialogue.nextDialogueLine);
                ReadCurrentLine();
                return;
            }

            if (_currentDialogue.randomizedDialogLines.Length > 0)
            {
                var randomIndex = Random.Range(0, _currentDialogue.randomizedDialogLines.Length);
                UpdateDialogState(_currentDialogue.randomizedDialogLines[randomIndex]);
                ReadCurrentLine();
                return;
            }

            if (_currentDialogue.objectReferringDialogLines.Length > 0)
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

        private void GeneratePlayerInputs(NarrationDialogLine line)
        {
            print("Notifying player choice");
            Notify(GameEvents.PlayerChoice, line.playerOptions);

            // for (var i = 0; i < line.playerOptions.Length; i++)
            // {
            //     var playerChoice = line.playerOptions[i];
            //     var playerChoiceNextAction = playerChoice.next;
            //     var buttonByType = playerChoice.type == ChoiceType.Button
            //         ? playerChoiceButton
            //         : playerInconsequentialChoice;
            //
            //     switch (playerChoice.type)
            //     {
            //         case ChoiceType.InconsequentialButton:
            //         case ChoiceType.Button:
            //             var button = Instantiate(buttonByType, playerChoiceButtonsContainer.transform);
            //             var position = button.transform.position;
            //             var width = button.GetComponent<RectTransform>().sizeDelta.x;
            //             
            //             position.x += (i - line.playerOptions.Length / 2) * width;
            //             position.y -= 50;
            //             
            //             button.transform.position = position;
            //             button.GetComponentInChildren<TextMeshProUGUI>().text = playerChoice.text;
            //             if (playerChoice.type == ChoiceType.Button)
            //                 button.GetComponent<PlayerInfoInput>().type = playerChoice.choiceDataType;
            //             
            //             button.GetComponent<Button>().onClick
            //                 .AddListener(() => OnPlayerChoiceButtonClick(playerChoiceNextAction));
            //             break;
            // TODO: handle text input for RIC intro
            //         case ChoiceType.TextInput:
            //             var textInput = Instantiate(playerTextInput, playerInputsContainer.transform);
            //             textInput.GetComponent<PlayerInfoInput>().type = playerChoice.choiceDataType;
            //             break;
            //     }
            // }
        }

        private void TriggerAnAct(GameEvents action)
        {
            switch (action)
            {
                case GameEvents.NextScene:
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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

        public void OnPlayerChoiceButtonClick(NarrationDialogLine nextLine)
        {
            foreach (var input in GameObject.FindGameObjectsWithTag("PlayerInput"))
                Destroy(input);

            UpdateDialogState(nextLine);
            HandlePlayerNameLines();
            ReadCurrentLine();
        }

        private void OnPlayerChoiceButtonClick(string buttonText)
        {
            StopAllCoroutines();
            _audioSource.Stop();

            // the same text rendered via the node - so comparison is safe
            var nextLine = _currentDialogue.playerOptions
                .First(option => option.text == buttonText).next;

            UpdateDialogState(nextLine);
            HandlePlayerNameLines();
            ReadCurrentLine();
        }

        public void OnNotify(GameEventData gameEventData)
        {
            print("DialogueManager: OnNotify " + gameEventData.name);

            try
            {
                var eventLine =
                    eventToDialogMap.dialogLineEvents.First(dialogLineEvent =>
                        dialogLineEvent.eventName == gameEventData.name);
                if (eventLine.dialogLine)
                {
                    TriggerLine(eventLine.dialogLine);
                    return;
                }
            }
            catch (InvalidOperationException e)
            {
                // ignored, this is fine and expected
            }

            switch (gameEventData.name)
            {
                case GameEvents.EnteredMuseum:
                    TriggerLine((NarrationDialogLine)gameEventData.data);
                    break;

                case GameEvents.PlayerClickOnChoice:
                    var choiceData = (EnrichedPlayerChoice)gameEventData.data;
                    if (choiceData.Choice == "YES")
                        try
                        {
                            // The state is already saved, just trigger the appropriate event
                            var interactionType = choiceData.OriginalInteraction.InteractableObjectType;
                            // var eventName = GetEventNameForInteractionType(interactionType);
                            Notify(GameEvents.PlayerEnrichedChoice, choiceData.OriginalInteraction);
                        }
                        catch (NullReferenceException error)
                        {
                            print(error);
                        }

                    OnPlayerChoiceButtonClick(choiceData.Choice);
                    break;

                case GameEvents.ClickOnNpc:
                case GameEvents.DoorClicked:
                case GameEvents.ArmchairClicked:
                case GameEvents.VaseClicked:
                case GameEvents.MirrorClicked:
                case GameEvents.PaintingClicked:
                case GameEvents.ShapeClicked:
                case GameEvents.DependentClicked:
                    var interactionData = (InteractionData)gameEventData.data;
                    InteractionStateService.Instance.SetCurrentInteraction(interactionData);
                    TriggerLine(interactionData.DialogLine);
                    break;
            }
        }

        private GameEvents GetEventNameForInteractionType(InteractableObjectType interactionType)
        {
            return interactionType switch
            {
                InteractableObjectType.Vase => GameEvents.VaseClicked,
                InteractableObjectType.Mirror => GameEvents.MirrorClicked,
                InteractableObjectType.Painting => GameEvents.PaintingClicked,
                InteractableObjectType.Armchair => GameEvents.ArmchairClicked,
                InteractableObjectType.Door => GameEvents.DoorClicked
            };
        }

        public void TriggerLine(NarrationDialogLine line)
        {
            if (_audioSource.isPlaying)
            {
                _triggeredDialogueLine = line;
                return;
            }

            UpdateDialogState(line);

            if (line.waitBeforeLine > 0)
                Invoke(nameof(ReadCurrentLine), line.waitBeforeLine);
            else
                ReadCurrentLine();
        }

        public void OnFastForwardDialogHold()
        {
            _audioSource.pitch = 2;
        }

        public void OnFastForwardDialogRelease()
        {
            _audioSource.pitch = 1;
            _audioSource.clip = null;
            StopAllCoroutines();
            OnDialogEnd();
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
                overlayImage.color = new Color(255, 255, 255, alpha);
                yield return null;
            }

            // // Ensure we end at exactly the target value
            // shadeSpriteRenderer.color = new Color(0, 0, 0, endAlpha);
            // containerSpriteRenderer.sortingOrder = fadeIn ? 3 : 7;
            //
            yield return new WaitForSeconds(1);
        }
    }
}