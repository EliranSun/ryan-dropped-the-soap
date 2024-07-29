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
            Invoke(nameof(OnDialogEnd), currentDialog.waitAfterLine);
        }

        private void ReadCurrentLine()
        {
            if (!currentDialog)
                return;

            subtitles.text = currentDialog.subtitles;

            if (currentDialog.line)
            {
                _audioSource.clip = currentDialog.line;
                _audioLowPassFilter.enabled = currentDialog.isMuffled;
                _audioSource.Play();
                StartCoroutine(CheckAudioEnd());
            }
            else
            {
                Invoke(nameof(OnDialogEnd), currentDialog.waitAfterLine);
            }
        }

        private void OnDialogEnd()
        {
            subtitles.text = "";

            if (currentDialog.nextDialogueLine)
            {
                TriggerAction();
                UpdateDialogState(currentDialog.nextDialogueLine);
                ReadCurrentLine();
                return;
            }

            if (_triggeredDialogueLine != null)
            {
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
            _audioSource.Stop();
            StopAllCoroutines();
            UpdateDialogState(line);
            Invoke(nameof(ReadCurrentLine), line.waitBeforeLine);
        }

        public void UpdateDialogState(DialogLineObject nextLineObject)
        {
            currentDialog = nextLineObject;
        }
    }
}