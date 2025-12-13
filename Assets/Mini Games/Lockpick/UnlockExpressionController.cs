using System;
using System.Linq;
using Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = System.Random;

namespace Mini_Games.Lockpick
{
    [Serializable]
    public class ExpressionVisual
    {
        public Image expressionImage;
        public Expression expression;
        public Expression[] matchingExpressions;
    }

    public class UnlockExpressionController : ObserverSubject, IPointerClickHandler
    {
        [Header("Expression Lock Pick Mini Game")] [SerializeField]
        private int gridSize = 3;

        [SerializeField] private GameObject gameContainer;
        [SerializeField] private TextMeshProUGUI expressionText;
        [SerializeField] private Image initExpression;
        [SerializeField] private ExpressionVisual[] gridImages;
        [SerializeField] private Image[] progressBar;

        private Expression _activeExpression;
        private bool _hasMouseMoved;
        private bool _isActive;
        private Vector3 _previousMousePosition;

        private int _progressBarIndex;

        private void Start()
        {
            gameContainer.SetActive(false);

            initExpression.gameObject.SetActive(true);

            foreach (var image in gridImages)
                image.expressionImage.gameObject.SetActive(false);

            // Initialize mouse tracking
            _previousMousePosition = Input.mousePosition;
            _hasMouseMoved = false;

            SetActiveExpression(GetRandomExpression());
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

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_progressBarIndex >= progressBar.Length) return;

            // Get the clicked expression name
            var clickedExpression = GetClickedExpression(eventData);
            var isCorrect = IsCorrectExpression(clickedExpression);
            Debug.Log($"Clicked expression: {clickedExpression}; is correct? {isCorrect}");

            if (!isCorrect)
                return;

            SetActiveExpression(GetRandomExpression());
            progressBar[_progressBarIndex].color = Color.black;
            _progressBarIndex++;

            if (_progressBarIndex >= progressBar.Length)
            {
                // win state
                StopAllCoroutines();
                gameContainer.SetActive(true);
            }
        }

        private ExpressionVisual GetClickedExpression(PointerEventData eventData)
        {
            if (gridImages == null || gridImages.Length == 0)
                return null;

            var screenPosition = eventData.position;
            var mouseX = screenPosition.x;
            var mouseY = screenPosition.y;
            var screenSize = new Vector2(Screen.width, Screen.height);
            var cellSize = screenSize / gridSize;

            // Calculate which grid cell was clicked (same logic as UpdateImageOpacity)
            var gridX = Mathf.Clamp(Mathf.FloorToInt(mouseX / cellSize.x), 0, gridSize - 1);
            var gridY = Mathf.Clamp(Mathf.FloorToInt((screenSize.y - mouseY) / cellSize.y), 0, gridSize - 1);

            // Convert 2D grid position to 1D array index
            var targetIndex = gridY * gridSize + gridX;

            // Ensure the target index is within bounds and return the GameObject name
            if (targetIndex >= 0 && targetIndex < gridImages.Length && gridImages[targetIndex] != null)
                return gridImages[targetIndex];

            return null;
        }

        private bool IsCorrectExpression(ExpressionVisual clickedExpression)
        {
            return clickedExpression.matchingExpressions.Contains(_activeExpression);
        }

        private static Expression GetRandomExpression()
        {
            var values = Enum.GetValues(typeof(Expression));
            var random = new Random();
            return (Expression)values.GetValue(random.Next(values.Length));
        }

        private void SetActiveExpression(Expression expression)
        {
            _activeExpression = expression;
            // expressionText.text = ExpressionTranslations.GetHebrewTranslation(expression);
            expressionText.text = expression.ToString().ToUpper();
        }

        private void OnFirstMouseMove()
        {
            if (!_isActive) _isActive = true;
            // initExpression.gameObject.SetActive(false);
            // foreach (var image in gridImages)
            //     image.gameObject.SetActive(true);
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
                        var color = gridImages[i].expressionImage.color;

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

                        gridImages[i].expressionImage.color = color;
                    }
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.MiniGameIndicationTrigger)
            {
                var eventMiniGameName = (MiniGameName)eventData.Data;
                if (eventMiniGameName == MiniGameName.LockPick)
                    gameContainer.SetActive(true);
            }
        }
    }
}