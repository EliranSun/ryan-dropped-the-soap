using System.Collections;
using TMPro;
using UnityEngine;

namespace Dialog.Scripts
{
    public class DialogBubbleBehavior : MonoBehaviour
    {
        [SerializeField] private float hoverDistance = 0.1f;
        [SerializeField] private float minSpeed = 0.5f;
        [SerializeField] private float maxSpeed = 1.5f;
        public string text;
        public GameEvents eventAfterChoice;

        private Vector3 _startPosition;
        private Transform _textCapsuleBackground;
        private TextMeshPro _textMeshPro;

        private void Start()
        {
            _textMeshPro = GetComponentInChildren<TextMeshPro>();
            _textCapsuleBackground = transform.GetChild(1);
            _startPosition = transform.localPosition;
            _textMeshPro.text = text;

            ChangeDimensionsBasedOnText();
            StartCoroutine(HoverAnimation());
        }

        private IEnumerator HoverAnimation()
        {
            while (true)
            {
                var speed = Random.Range(minSpeed, maxSpeed);
                var targetPosition = _startPosition + Vector3.up * hoverDistance;
                yield return MoveToPosition(targetPosition, speed);

                targetPosition = _startPosition - Vector3.up * hoverDistance;
                yield return MoveToPosition(targetPosition, speed);
            }
        }

        private IEnumerator MoveToPosition(Vector3 targetPosition, float speed)
        {
            while (Vector3.Distance(transform.localPosition, targetPosition) > 0.01f)
            {
                transform.localPosition =
                    Vector3.MoveTowards(transform.localPosition, targetPosition, speed * Time.deltaTime);
                yield return null;
            }
        }

        private void ChangeDimensionsBasedOnText()
        {
            // Define the maximum number of characters per line
            var maxCharsPerLine = 30; // Adjust this value as needed

            // Break the text into lines
            var words = text.Split(' ');
            var newText = "";
            var currentLine = "";

            foreach (var word in words)
                if ((currentLine + word).Length > maxCharsPerLine)
                {
                    newText += currentLine + "\n";
                    currentLine = word + " ";
                }
                else
                {
                    currentLine += word + " ";
                }

            newText += currentLine.TrimEnd(); // Add the last line

            // Update the TextMeshPro text
            _textMeshPro.text = newText;

            var lineWidth = _textMeshPro.fontSize * 0.3f; // Adjust line height multiplier as needed
            var lineHeight = _textMeshPro.fontSize * 1f; // Adjust line height multiplier as needed
            var lineCount = newText.Split('\n').Length;
            var newHeight = lineHeight * lineCount;

            // Assuming the capsule's width should remain constant, adjust only the height
            var newScale = _textCapsuleBackground.localScale;
            newScale.x = lineWidth / _textCapsuleBackground.GetComponent<SpriteRenderer>().sprite.bounds.size.x;
            newScale.y = newHeight / _textCapsuleBackground.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
            _textCapsuleBackground.localScale = newScale;
        }
    }
}