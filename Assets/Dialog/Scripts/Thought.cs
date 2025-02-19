using TMPro;
using UnityEngine;

namespace Dialog.Scripts
{
    public class Thought : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textContainer;
        private NarrationDialogLine _nextLine;
        private PlayerDataEnum _playerDataType;
        private bool _spokeMind;
        private ThoughtsManager _thoughtsManager;

        private void Start()
        {
            _thoughtsManager = FindFirstObjectByType<ThoughtsManager>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Saying box") && !_spokeMind)
            {
                _spokeMind = true;
                Invoke(nameof(Speak), 1);
            }
        }

        private void Speak()
        {
            _thoughtsManager.OnSpeak(textContainer.text, _nextLine, _playerDataType);
        }

        public void SetThought(string newText)
        {
            textContainer.text = newText;
        }

        public void SetNextLine(NarrationDialogLine nextDialogLine)
        {
            _nextLine = nextDialogLine;
        }

        public void SetChoicePlayerDataType(PlayerDataEnum playerDataType)
        {
            _playerDataType = playerDataType;
        }
    }
}