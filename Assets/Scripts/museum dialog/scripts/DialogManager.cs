using System.Collections;
using System.Linq;
using Character_Creator.scripts;
using Dialog.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace museum_dialog.scripts
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
        private bool _asyncLinesReady;
        private AudioSource _audioSource;
        private NarrationDialogLine _currentDialogue;
        private int _forgettingGuideCount;
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

        private IEnumerator HandlePlayerNameLinesAndStartRead()
        {
            var playerName = PlayerData.GetPlayerName();
            var playerGender = PlayerData.GetPlayerGender();

            if (playerName != "" && playerGender != CharacterType.None && !PrefetchDialogLines.Instance.isFetched)
                yield return StartCoroutine(PrefetchDialogLines.Instance.FetchAndPopulatePlayerLines());

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

            print(_currentDialogue);
            var line = GetLineByGender(_currentDialogue);

            narratorText.text = line.text
                .Replace("{playerName}", PlayerData.GetPlayerName())
                .Replace("{partnerName}", PlayerData.GetPartnerName());

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
            var gender = PlayerData.GetPlayerGender();
            return dialogueLineObject.voicedLines.First(voicedLine =>
            {
                if (voicedLine.gender == CharacterType.NonBinary || voicedLine.gender == CharacterType.None)
                    return true;

                return voicedLine.gender == gender;
            });
        }

        private void OnDialogEnd()
        {
            Notify(GameEvents.LineNarrationEnd, _currentDialogue.actorName);
            narratorText.text = "";
            HandlePlayerNameLines();

            if (_currentDialogue.playerOptions.Length > 0 && NoPlayerInputsExist())
            {
                GeneratePlayerInputs(_currentDialogue);
                return;
            }

            if (_currentDialogue.actionAfterLine != GameEvents.None)
                ActAfterLine(_currentDialogue.actionAfterLine);

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
            //
            //         case ChoiceType.TextInput:
            //             var textInput = Instantiate(playerTextInput, playerInputsContainer.transform);
            //             textInput.GetComponent<PlayerInfoInput>().type = playerChoice.choiceDataType;
            //             break;
            //     }
            // }
        }

        private void ActAfterLine(GameEvents action)
        {
            switch (action)
            {
                case GameEvents.CountForgettingGuide:
                    _forgettingGuideCount++;
                    break;

                case GameEvents.NextScene:
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    break;

                case GameEvents.None:
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
            // the same text rendered via the node - so comparison is safe
            var nextLine = _currentDialogue.playerOptions
                .First(option => option.text == buttonText).next;

            UpdateDialogState(nextLine);
            HandlePlayerNameLines();
            ReadCurrentLine();
        }

        public void OnNotify(GameEventData gameEventData)
        {
            switch (gameEventData.name)
            {
                case GameEvents.EnteredMuseum:
                    TriggerLine((NarrationDialogLine)gameEventData.data);
                    break;

                case GameEvents.PlayerClickOnChoice:
                    var choiceText = (string)gameEventData.data;
                    if (choiceText == "YES" && _potentialSelectedInteractableObject != InteractableObjectName.Unknown)
                    {
                        var existingSelectedObjects =
                            PlayerPrefs.GetString(_potentialSelectedInteractableObjectType.ToString());
                        print(existingSelectedObjects);

                        PlayerPrefs.SetString(_potentialSelectedInteractableObjectType.ToString(),
                            _potentialSelectedInteractableObject.ToString());
                    }

                    OnPlayerChoiceButtonClick(choiceText);
                    break;

                case GameEvents.ClickOnNpc:
                case GameEvents.DoorClicked:
                case GameEvents.ArmchairClicked:
                case GameEvents.VaseClicked:
                case GameEvents.MirrorClicked:
                case GameEvents.PaintingClicked:
                    var interactionData = (InteractionData)gameEventData.data;
                    _potentialSelectedInteractableObject = interactionData.InteractableObjectName;
                    _potentialSelectedInteractableObjectType = interactionData.InteractableObjectType;
                    TriggerLine(interactionData.DialogLine);
                    break;
            }
        }

        public void TriggerLine(NarrationDialogLine line)
        {
            if (_audioSource.isPlaying)
            {
                _triggeredDialogueLine = line;
                return;
            }

            UpdateDialogState(line);
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
    }
}