using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dialog.Scripts {
    [RequireComponent(typeof(AudioSource))]
    public class DialogSystem : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI subtitles;
        [SerializeField] private DialogLineObject currentDialog;

        private AudioSource _audioSource;
        private DialogLineObject _triggeredDialogueLine;

        public static DialogSystem Instance { get; private set; }

        private void Awake() {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;

            _audioSource = GetComponent<AudioSource>();
        }

        private void Start() {
            UpdateDialogState(currentDialog);
            ReadCurrentLine();
        }

        private IEnumerator CheckAudioEnd() {
            yield return new WaitWhile(() => _audioSource.isPlaying);

            print($"Audio stopped playing, Invoking on dialog end after {currentDialog.waitAfterLine}");
            Invoke(nameof(OnDialogEnd), currentDialog.waitAfterLine);
        }

        private void ReadCurrentLine() {
            if (!currentDialog) {
                print($"Attempt playing but not current dialog {currentDialog}");
                return;
            }

            subtitles.text = currentDialog.subtitles;

            if (currentDialog.line) {
                print($"Audio play for {currentDialog}");
                _audioSource.clip = currentDialog.line;
                _audioSource.Play();
                StartCoroutine(CheckAudioEnd());
            }
            else {
                print($"no current line, Invoking on dialog end after {currentDialog.waitAfterLine}");
                Invoke(nameof(OnDialogEnd), currentDialog.waitAfterLine);
            }
        }

        private void OnDialogEnd() {
            subtitles.text = "";

            if (currentDialog.nextDialogueLine) {
                print($"Next dialog line detected, reading {currentDialog.nextDialogueLine}");
                TriggerAction();
                UpdateDialogState(currentDialog.nextDialogueLine);
                ReadCurrentLine();
                return;
            }

            if (_triggeredDialogueLine != null) {
                print($"There is a pending triggered dialog line, reading {_triggeredDialogueLine}");
                TriggerAction();
                UpdateDialogState(_triggeredDialogueLine);
                _triggeredDialogueLine = null;
                ReadCurrentLine();
                return;
            }

            TriggerAction();
        }

        private void TriggerAction() {
            if (currentDialog.afterLineAction != DialogAction.None) {
                print($"Handling after line action {currentDialog.afterLineAction}");
                HandleAction(currentDialog.afterLineAction);
            }
        }

        private void HandleAction(DialogAction actionName) {
            switch (actionName) {
                case DialogAction.NextLevel:
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    break;

                case DialogAction.RestartLevel:
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    break;

                case DialogAction.EndZoom:
                    Zoom.Instance.endSize = 40;
                    break;
            }
        }

        public void TriggerLine(DialogLineObject line) {
            if (_audioSource.isPlaying) {
                print($"Line triggered {line} but audio is playing {currentDialog}. holding");
                _triggeredDialogueLine = line;
                return;
            }

            print($"Line triggered {line} and nothing is playing. reading.");
            UpdateDialogState(line);
            ReadCurrentLine();
        }

        public void UpdateDialogState(DialogLineObject nextLineObject) {
            currentDialog = nextLineObject;
        }
    }
}