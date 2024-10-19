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

        private Vector3 _startPosition;
        public string text;
        private TextMeshPro _textMeshPro;
        private Transform _textCapsuleBackground;

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
                float speed = Random.Range(minSpeed, maxSpeed);
                Vector3 targetPosition = _startPosition + Vector3.up * hoverDistance;
                yield return MoveToPosition(targetPosition, speed);

                targetPosition = _startPosition - Vector3.up * hoverDistance;
                yield return MoveToPosition(targetPosition, speed);
            }
        }

        private IEnumerator MoveToPosition(Vector3 targetPosition, float speed)
        {
            while (Vector3.Distance(transform.localPosition, targetPosition) > 0.01f)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, speed * Time.deltaTime);
                yield return null;
            }
        }

        private void ChangeDimensionsBasedOnText()
        {
            // Define the maximum number of characters per line
            int maxCharsPerLine = 30; // Adjust this value as needed

            // Break the text into lines
            string[] words = text.Split(' ');
            string newText = "";
            string currentLine = "";

            foreach (string word in words)
            {
                if ((currentLine + word).Length > maxCharsPerLine)
                {
                    newText += currentLine + "\n";
                    currentLine = word + " ";
                }
                else
                {
                    currentLine += word + " ";
                }
            }

            newText += currentLine.TrimEnd(); // Add the last line

            // Update the TextMeshPro text
            _textMeshPro.text = newText;

            // Adjust the size of the text capsule background based on the number of lines
            float lineHeight = _textMeshPro.fontSize * 1.2f; // Adjust line height multiplier as needed
            int lineCount = newText.Split('\n').Length;
            float newHeight = lineHeight * lineCount;

            // Assuming the capsule's width should remain constant, adjust only the height
            Vector3 newScale = _textCapsuleBackground.localScale;
            newScale.y = newHeight / _textCapsuleBackground.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
            _textCapsuleBackground.localScale = newScale;
        }
    }
}
