using TMPro;
using UnityEngine;

namespace Dialog
{
    public class Thought : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textContainer;
        public int destroyTime;
        private NarrationDialogLine _nextLine;
        private PlayerDataEnum _playerDataType;
        private int _score;
        private bool _spokeMind;
        private ThoughtsManager _thoughtsManager;

        private void Start()
        {
            _thoughtsManager = FindFirstObjectByType<ThoughtsManager>();
            if (destroyTime > 0) Destroy(gameObject, destroyTime);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Saying box") && !_spokeMind)
            {
                _spokeMind = true;
                Speak();
            }
        }

        private void Speak()
        {
            _thoughtsManager.OnSpeak(textContainer.text, _nextLine, _playerDataType, _score);
        }

        public void SetThought(string newText)
        {
            textContainer.text = newText;
        }

        public void SetNextLine(NarrationDialogLine nextDialogLine)
        {
            _nextLine = nextDialogLine;
        }

        public void SetThoughtScore(int score)
        {
            _score = score;
        }

        public void SetChoicePlayerDataType(PlayerDataEnum playerDataType)
        {
            _playerDataType = playerDataType;
        }
    }
}