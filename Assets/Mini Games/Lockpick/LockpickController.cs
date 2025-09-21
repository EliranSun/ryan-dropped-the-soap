using UnityEngine;
using UnityEngine.UI;

namespace Mini_Games.Lockpick
{
    public class LockpickController : MonoBehaviour
    {
        [Header("Grid Configuration")] [SerializeField]
        private int gridSize = 3;

        [SerializeField] private float cellSize = 100f;
        [SerializeField] private float spacing = 10f;

        [Header("Grid Container")] [SerializeField]
        private RectTransform gridContainer;

        [Header("Grid Images")] [SerializeField]
        private Image[] gridImages;

        [Header("Movement Settings")] [SerializeField]
        private float movementSpeed = 200f;

        [SerializeField] private float maxMovementDistance = 50f;
        [SerializeField] private float mouseSensitivity = 1f;

        [Header("Focus Settings")] [SerializeField]
        private float maxFocusDistance = 100f;

        [SerializeField] private AnimationCurve opacityCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private int centerIndex;
        private Vector2 gridOffset = Vector2.zero;
        private bool isMouseInitialized;
        private Vector2 lastMousePosition;

        private void Start()
        {
            InitializeGrid();
        }

        private void Update()
        {
            HandleInput();
            UpdateGridPosition();
            UpdateImageOpacity();
        }

        private void InitializeGrid()
        {
            // Validate grid setup
            var expectedImageCount = gridSize * gridSize;
            if (gridImages == null || gridImages.Length != expectedImageCount)
            {
                Debug.LogError(
                    $"Grid images array must contain exactly {expectedImageCount} images for a {gridSize}x{gridSize} grid!");
                return;
            }

            if (gridContainer == null)
            {
                Debug.LogError("Grid Container RectTransform is not assigned!");
                return;
            }

            // Calculate center index (for odd grid sizes, this will be the true center)
            centerIndex = gridSize * gridSize / 2;

            // Position images in grid layout and set visibility
            PositionImagesInGrid();

            Debug.Log($"Initialized {gridSize}x{gridSize} grid. Center image at index {centerIndex} is visible.");
        }

        private void PositionImagesInGrid()
        {
            // Calculate grid dimensions
            var totalGridWidth = gridSize * cellSize + (gridSize - 1) * spacing;
            var totalGridHeight = gridSize * cellSize + (gridSize - 1) * spacing;

            // Calculate starting position (top-left corner of the grid)
            var startX = -totalGridWidth / 2f + cellSize / 2f;
            var startY = totalGridHeight / 2f - cellSize / 2f;

            for (var i = 0; i < gridImages.Length; i++)
            {
                if (gridImages[i] == null) continue;

                // Calculate grid coordinates
                var coordinates = GetCoordinatesFromIndex(i);
                var x = coordinates.x;
                var y = coordinates.y;

                // Calculate world position
                var posX = startX + x * (cellSize + spacing);
                var posY = startY - y * (cellSize + spacing);

                // Set the image's RectTransform properties
                var rectTransform = gridImages[i].rectTransform;

                // Parent to grid container if not already
                if (rectTransform.parent != gridContainer)
                    rectTransform.SetParent(gridContainer, false);

                // Set position
                rectTransform.anchoredPosition = new Vector2(posX, posY);

                // Set anchors to center
                rectTransform.anchorMin = Vector2.one * 0.5f;
                rectTransform.anchorMax = Vector2.one * 0.5f;
                rectTransform.pivot = Vector2.one * 0.5f;

                // Make all images visible but set initial opacity
                gridImages[i].gameObject.SetActive(true);
            }
        }

        private void HandleInput()
        {
            var inputMovement = Vector2.zero;

            // Handle keyboard input (arrow keys)
            if (Input.GetKey(KeyCode.LeftArrow)) inputMovement.x -= 1f;
            if (Input.GetKey(KeyCode.RightArrow)) inputMovement.x += 1f;
            if (Input.GetKey(KeyCode.UpArrow)) inputMovement.y += 1f;
            if (Input.GetKey(KeyCode.DownArrow)) inputMovement.y -= 1f;

            // Handle joystick input
            var joystickInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            inputMovement += joystickInput;

            // Handle mouse input
            var currentMousePosition = Input.mousePosition;
            if (!isMouseInitialized)
            {
                lastMousePosition = currentMousePosition;
                isMouseInitialized = true;
            }
            else
            {
                var mouseDelta = ((Vector2)currentMousePosition - lastMousePosition) * mouseSensitivity;
                inputMovement += new Vector2(mouseDelta.x, mouseDelta.y) * Time.deltaTime;
                lastMousePosition = currentMousePosition;
            }

            // Apply movement (inverse direction)
            if (inputMovement.magnitude > 0.01f)
            {
                var movement = -inputMovement * (movementSpeed * Time.deltaTime);
                gridOffset += movement;

                // Clamp movement to prevent going too far
                gridOffset = Vector2.ClampMagnitude(gridOffset, maxMovementDistance);
            }
        }

        private void UpdateGridPosition()
        {
            if (gridContainer == null) return;

            // Apply the offset to the grid container
            gridContainer.anchoredPosition = gridOffset;
        }

        private void UpdateImageOpacity()
        {
            var screenCenter = Vector2.zero; // Center of the screen/container

            for (var i = 0; i < gridImages.Length; i++)
            {
                if (gridImages[i] == null) continue;

                // Get the world position of the image relative to screen center
                var imageWorldPos = gridImages[i].rectTransform.anchoredPosition + gridOffset;
                var distanceFromCenter = Vector2.Distance(imageWorldPos, screenCenter);

                // Calculate opacity based on distance from center
                var normalizedDistance = Mathf.Clamp01(distanceFromCenter / maxFocusDistance);
                var opacity = opacityCurve.Evaluate(1f - normalizedDistance); // Invert so closer = more opaque

                // Apply opacity to the image
                var color = gridImages[i].color;
                color.a = opacity;
                gridImages[i].color = color;
            }
        }

        // Helper method to convert 2D coordinates to 1D array index
        private int GetIndexFromCoordinates(int x, int y)
        {
            return y * gridSize + x;
        }

        // Helper method to convert 1D array index to 2D coordinates
        private Vector2Int GetCoordinatesFromIndex(int index)
        {
            return new Vector2Int(index % gridSize, index / gridSize);
        }

        // Method to show/hide specific image by coordinates
        public void SetImageVisibility(int x, int y, bool visible)
        {
            var index = GetIndexFromCoordinates(x, y);
            if (index >= 0 && index < gridImages.Length && gridImages[index] != null)
                gridImages[index].gameObject.SetActive(visible);
        }

        // Method to show/hide specific image by index
        public void SetImageVisibility(int index, bool visible)
        {
            if (index >= 0 && index < gridImages.Length && gridImages[index] != null)
                gridImages[index].gameObject.SetActive(visible);
        }
    }
}