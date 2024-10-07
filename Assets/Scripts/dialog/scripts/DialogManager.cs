using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace dialog.scripts
{
    [RequireComponent(typeof(AudioSource))]
    // [RequireComponent(typeof(DialogueStateChanger))]
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

        private DialogueLineObject _currentDialogue;

        private PlayerData _player;

        private DialogueLineObject _triggeredDialogueLine;
        // public static DialogueManager Instance { get; private set; }

        private void Awake()
        {
            // if (Instance != null && Instance != this)
            //     Destroy(this);
            // else
            //     Instance = this;

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
                StartCoroutine(CheckAudioEnd());
            }
            else
            {
                Invoke(nameof(OnDialogEnd), _currentDialogue.wait);
            }
        }

        private static VoicedLine GetLineByGender(DialogueLineObject dialogueLineObject)
        {
            var gender = PlayerData.GetPlayerGender();
            print($"Player Gender: {gender}");

            return dialogueLineObject.voicedLines.First(voicedLine =>
            {
                if (voicedLine.gender == CharacterType.NonBinary || voicedLine.gender == CharacterType.None)
                    return true;

                return voicedLine.gender == gender;
            });
        }

        private void OnDialogEnd()
        {
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

        private void GeneratePlayerInputs(DialogueLineObject line)
        {
            for (var i = 0; i < line.playerOptions.Length; i++)
            {
                var playerChoice = line.playerOptions[i];
                var buttonByType = playerChoice.type == ChoiceType.Button
                    ? playerChoiceButton
                    : playerInconsequentialChoice;

                switch (playerChoice.type)
                {
                    case ChoiceType.InconsequentialButton:
                    case ChoiceType.Button:
                        var button = Instantiate(buttonByType, playerChoiceButtonsContainer.transform);
                        var position = button.transform.position;
                        var width = button.GetComponent<RectTransform>().sizeDelta.x;

                        position.x += (i - line.playerOptions.Length / 2) * width;
                        position.y -= 50;

                        button.transform.position = position;
                        button.GetComponentInChildren<TextMeshProUGUI>().text = playerChoice.text;
                        if (playerChoice.type == ChoiceType.Button)
                            button.GetComponent<PlayerInfoInput>().type = playerChoice.choiceDataType;

                        break;

                    case ChoiceType.TextInput:
                        var textInput = Instantiate(playerTextInput, playerInputsContainer.transform);
                        textInput.GetComponent<PlayerInfoInput>().type = playerChoice.choiceDataType;
                        break;
                }
            }
        }

        private void ActAfterLine(GameEvents action)
        {
            switch (action)
            {
                case GameEvents.NextScene:
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    break;

                case GameEvents.None:
                    break;

                default:
                    // print($"Notifying action {action}");
                    Notify(action);
                    break;
            }
        }

        public void OnPlayerChoiceButtonClick(string buttonText)
        {
            foreach (var input in GameObject.FindGameObjectsWithTag("PlayerInput"))
                Destroy(input);

            // the same text rendered via the node - so comparison is safe
            var nextLine = _currentDialogue.playerOptions
                .First(option => option.text == buttonText).next;

            UpdateDialogState(nextLine);
            HandlePlayerNameLines();
            ReadCurrentLine();
        }

        public void TriggerLine(DialogueLineObject line)
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

        public void UpdateDialogState(DialogueLineObject nextLineObject)
        {
            _currentDialogue = nextLineObject;
        }
    }
}