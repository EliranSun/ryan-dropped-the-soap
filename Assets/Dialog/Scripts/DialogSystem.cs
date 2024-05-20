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

            Invoke(nameof(OnDialogEnd), currentDialog.waitAfterLine);
        }

        private void ReadCurrentLine() {
            if (!currentDialog)
                return;

            subtitles.text = currentDialog.subtitles;

            if (currentDialog.line) {
                _audioSource.clip = currentDialog.line;
                _audioSource.Play();
                StartCoroutine(CheckAudioEnd());
            }
            else {
                Invoke(nameof(OnDialogEnd), currentDialog.waitAfterLine);
            }
        }

        private void OnDialogEnd() {
            subtitles.text = "";

            if (currentDialog.nextDialogueLine) {
                UpdateDialogState(currentDialog.nextDialogueLine);
                ReadCurrentLine();
                return;
            }

            if (_triggeredDialogueLine != null) {
                UpdateDialogState(_triggeredDialogueLine);
                _triggeredDialogueLine = null;
                ReadCurrentLine();
                return;
            }

            if (currentDialog.afterLineAction != DialogAction.None)
                HandleAction(currentDialog.afterLineAction);
        }

        private void HandleAction(DialogAction actionName) {
            switch (actionName) {
                case DialogAction.NextLevel:
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    break;

                case DialogAction.RestartLevel:
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    break;
            }
        }

        public void TriggerLine(DialogLineObject line) {
            if (_audioSource.isPlaying) {
                _triggeredDialogueLine = line;
                return;
            }

            UpdateDialogState(line);
            ReadCurrentLine();
        }

        public void UpdateDialogState(DialogLineObject nextLineObject) {
            currentDialog = nextLineObject;
        }
    }
}