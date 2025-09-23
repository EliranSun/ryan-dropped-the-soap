using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Mini_Games.Lockpick
{
    [Serializable]
    public enum Expression
    {
        Sternness,
        Indignation,
        Anger,
        Rage,
        Disdain,
        Aversion,
        Disgust,
        Revulsion,
        Concern,
        Anxiety,
        Fear,
        Terror
    }

    public class UnlockExpressionController : MonoBehaviour
    {
        [SerializeField] private int gridSize = 3;

        [SerializeField] private TextMeshProUGUI expressionText;
        [SerializeField] private Image initExpression;
        [SerializeField] private Image[] gridImages;
        private bool _hasMouseMoved;

        private bool _isActive;
        private Vector3 _previousMousePosition;

        private void Start()
        {
            initExpression.gameObject.SetActive(true);

            foreach (var image in gridImages)
                image.gameObject.SetActive(false);

            // Initialize mouse tracking
            _previousMousePosition = Input.mousePosition;
            _hasMouseMoved = false;

            expressionText.text = GetRandomExpression().ToString().ToUpper();
        }

        private void Update()
        {
            // Check for first mouse movement
            if (!_hasMouseMoved)
            {
                var currentMousePosition = Input.mousePosition;
                if (Vector3.Distance(currentMousePosition, _previousMousePosition) > 0.1f)
                {
                    _hasMouseMoved = true;
                    OnFirstMouseMove();
                }
            }

            UpdateImageOpacity();
        }

        private void OnMouseDown()
        {
            print("CLICK");
        }

        /// <summary>
        ///     Returns a random expression from the Expression enum
        /// </summary>
        /// <returns>A random Expression enum value</returns>
        public static Expression GetRandomExpression()
        {
            var values = Enum.GetValues(typeof(Expression));
            var random = new Random();
            return (Expression)values.GetValue(random.Next(values.Length));
        }

        private void OnFirstMouseMove()
        {
            if (!_isActive)
            {
                _isActive = true;
                initExpression.gameObject.SetActive(false);
                foreach (var image in gridImages)
                    image.gameObject.SetActive(true);
            }
        }

        private void UpdateImageOpacity()
        {
            if (gridImages == null) return;

            var mousePosition = Input.mousePosition;
            var mouseX = mousePosition.x;
            var mouseY = mousePosition.y;
            var screenSize = new Vector2(Screen.width, Screen.height);
            var cellSize = screenSize / gridSize;

            // Calculate which grid cell the mouse is in
            var gridX = Mathf.Clamp(Mathf.FloorToInt(mouseX / cellSize.x), 0, gridSize - 1);
            var gridY = Mathf.Clamp(Mathf.FloorToInt((screenSize.y - mouseY) / cellSize.y), 0, gridSize - 1);

            // Convert 2D grid position to 1D array index
            var targetIndex = gridY * gridSize + gridX;

            // Ensure the target index is within bounds
            if (targetIndex >= 0 && targetIndex < gridImages.Length)
                // Update opacity for all images
                for (var i = 0; i < gridImages.Length; i++)
                    if (gridImages[i] != null)
                    {
                        var color = gridImages[i].color;

                        if (i == targetIndex)
                        {
                            // Lerp towards full opacity for the target image
                            color.a = Mathf.Lerp(color.a, 1f, Time.deltaTime * 8f);
                            // Snap to 1 when close enough
                            if (color.a > 0.9f) color.a = 1f;
                        }
                        else
                        {
                            // Lerp towards transparent for other images
                            color.a = Mathf.Lerp(color.a, 0f, Time.deltaTime * 8f);
                            // Snap to 0 when close enough
                            if (color.a < 0.1f) color.a = 0f;
                        }

                        gridImages[i].color = color;
                    }
        }
    }
}