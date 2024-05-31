using System.Collections;
using TMPro;
using UnityEngine;

namespace Dialog.Scripts
{
    [RequireComponent(typeof(AudioLowPassFilter))]
    [RequireComponent(typeof(AudioSource))]
    public class DialogSystem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI subtitles;
        [SerializeField] private DialogLineObject currentDialog;
        private AudioLowPassFilter _audioLowPassFilter;

        private AudioSource _audioSource;
        private DialogLineObject _triggeredDialogueLine;

        public static DialogSystem Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
        }

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioLowPassFilter = GetComponent<AudioLowPassFilter>();

            UpdateDialogState(currentDialog);
            ReadCurrentLine();
        }

        private IEnumerator CheckAudioEnd()
        {
            yield return new WaitWhile(() => _audioSource.isPlaying);

            // print($"Audio stopped playing, Invoking on dialog end after {currentDialog.waitAfterLine}");
            Invoke(nameof(OnDialogEnd), currentDialog.waitAfterLine);
        }

        private void ReadCurrentLine()
        {
            if (!currentDialog)
                // print($"Attempt playing but not current dialog {currentDialog}");
                return;

            subtitles.text = currentDialog.subtitles;

            if (currentDialog.line)
            {
                // print($"Audio play for {currentDialog}");
                _audioSource.clip = currentDialog.line;
                _audioLowPassFilter.enabled = currentDialog.isMuffled;
                _audioSource.Play();
                StartCoroutine(CheckAudioEnd());
            }
            else
            {
                // print($"no current line, Invoking on dialog end after {currentDialog.waitAfterLine}");
                Invoke(nameof(OnDialogEnd), currentDialog.waitAfterLine);
            }
        }

        private void OnDialogEnd()
        {
            subtitles.text = "";

            if (currentDialog.nextDialogueLine)
            {
                // print($"Next dialog line detected, reading {currentDialog.nextDialogueLine}");
                TriggerAction();
                UpdateDialogState(currentDialog.nextDialogueLine);
                ReadCurrentLine();
                return;
            }

            if (_triggeredDialogueLine != null)
            {
                // print($"There is a pending triggered dialog line, reading {_triggeredDialogueLine}");
                TriggerAction();
                UpdateDialogState(_triggeredDialogueLine);
                _triggeredDialogueLine = null;
                ReadCurrentLine();
                return;
            }

            TriggerAction();
        }

        private void TriggerAction()
        {
            if (currentDialog.afterLineAction != DialogAction.None)
                // print($"Handling after line action {currentDialog.afterLineAction}");
                HandleAction(currentDialog.afterLineAction);
        }

        private void HandleAction(DialogAction actionName)
        {
            switch (actionName)
            {
                case DialogAction.NextLevel:
                    LevelManager.NextLevel();
                    break;

                case DialogAction.RestartLevel:
                    LevelManager.RestartLevel();
                    break;

                case DialogAction.ZoomOut:
                    Zoom.Instance.endSize = 40;
                    break;
            }
        }

        public void TriggerLine(DialogLineObject line)
        {
            // if (_audioSource.isPlaying) {
            //     // print($"Line triggered {line} but audio is playing {currentDialog}. holding");
            //     _triggeredDialogueLine = line;
            //     return;
            // }

            // print($"Line triggered {line} and nothing is playing. reading.");
            _audioSource.Stop();
            StopAllCoroutines();
            UpdateDialogState(line);
            Invoke(nameof(ReadCurrentLine), 3);
        }

        public void UpdateDialogState(DialogLineObject nextLineObject)
        {
            currentDialog = nextLineObject;
        }
    }
}