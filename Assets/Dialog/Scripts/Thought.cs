using TMPro;
using UnityEngine;

namespace Dialog.Scripts
{
    public class Thought : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textContainer;
        private NarrationDialogLine _nextLine;
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
                print("SPOKE HIS MIND!");
                Invoke(nameof(Speak), 1);
            }
        }

        private void Speak()
        {
            _thoughtsManager.RemoveThoughts();
            _thoughtsManager.OnSpeak(textContainer.text, _nextLine);
            Invoke(nameof(RemoveSaying), 4);
        }

        public void RemoveSaying()
        {
            _thoughtsManager.RemoveSayings();
        }

        public void SetThought(string newText)
        {
            textContainer.text = newText;
        }

        public void SetNextLine(NarrationDialogLine nextDialogLine)
        {
            _nextLine = nextDialogLine;
        }
    }
}