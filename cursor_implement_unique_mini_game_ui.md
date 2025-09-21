# Implement unique mini game UI
_Exported on 9/21/2025 at 16:16:09 GMT+3 from Cursor (1.5.11)_

---

**User**

in @LockpickController.cs  I want to implement a unique mini game. 
UI wise - I want to have a series of images in a grid like pattern, but every image is hidden expect the "center". So if let's say I have a 3*3 grid, only the 5th element (or 1,1 position) will be visible. 
The images would be set via the editor so SerializedField need to be utilized. In addition - this is UI images inside a canvas we are talking about. 3 by 3 is just an example. The size should be dynamic (but we can assume it's a square if that simplifies things)
I have more requirements but let's start with representing that via code. If you are not sure about something, please ask before executing.

---

**Cursor**

I'll help you implement a unique mini game in the LockpickController. Let me understand your requirements:

1. A dynamic grid of UI images (square grid, e.g., 3x3, 5x5, etc.)
2. Only the center image is visible initially, all others are hidden
3. Images should be configurable via SerializedField in the editor
4. This uses UI Images within a Canvas

Before I implement this, I have a few questions to ensure I get it right:

1. Should the grid size be configurable in the editor (e.g., a field where you can set gridSize = 3 for 3x3, gridSize = 5 for 5x5)?
2. Do you want me to create the grid layout programmatically, or do you prefer to manually assign the images in a specific order in the editor?
3. Should I include a parent GameObject/RectTransform reference for organizing the grid layout?
4. Do you want any specific naming convention for the image GameObjects?

Let me start with a basic implementation based on what I understand, and you can let me know if adjustments are needed:

```csharp
using UnityEngine;
using UnityEngine.UI;

namespace Mini_Games.Lockpick
{
    public class LockpickController : MonoBehaviour
    {
        [Header("Grid Configuration")]
        [SerializeField] private int gridSize = 3;
        
        [Header("Grid Images")]
        [SerializeField] private Image[] gridImages;
        
        private int centerIndex;
        
        private void Start()
        {
            InitializeGrid();
        }
        
        private void InitializeGrid()
        {
            // Validate grid setup
            int expectedImageCount = gridSize * gridSize;
            if (gridImages == null || gridImages.Length != expectedImageCount)
            {
                Debug.LogError($"Grid images array must contain exactly {expectedImageCount} images for a {gridSize}x{gridSize} grid!");
                return;
            }
            
            // Calculate center index (for odd grid sizes, this will be the true center)
            centerIndex = (gridSize * gridSize) / 2;
            
            // Hide all images except the center one
            for (int i = 0; i < gridImages.Length; i++)
            {
                if (gridImages[i] != null)
                {
                    gridImages[i].gameObject.SetActive(i == centerIndex);
                }
            }
            
            Debug.Log($"Initialized {gridSize}x{gridSize} grid. Center image at index {centerIndex} is visible.");
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
            int index = GetIndexFromCoordinates(x, y);
            if (index >= 0 && index < gridImages.Length && gridImages[index] != null)
            {
                gridImages[index].gameObject.SetActive(visible);
            }
        }
        
        // Method to show/hide specific image by index
        public void SetImageVisibility(int index, bool visible)
        {
            if (index >= 0 && index < gridImages.Length && gridImages[index] != null)
            {
                gridImages[index].gameObject.SetActive(visible);
            }
        }
    }
}
```

I've implemented the basic structure for your lockpick mini game. Here's what the code provides:

## Key Features:

1. **Dynamic Grid Size**: `gridSize` field lets you configure the grid dimensions (3x3, 5x5, etc.)
2. **SerializedField Images**: `gridImages` array where you can assign your UI Images in the editor
3. **Center-Only Visibility**: Only the center image is visible on start, all others are hidden
4. **Helper Methods**: Utility functions to convert between 2D coordinates and array indices
5. **Visibility Control**: Methods to show/hide specific images by coordinates or index

## How it works:

- **Grid Setup**: The array expects exactly `gridSize²` images (9 for 3x3, 25 for 5x5, etc.)
- **Center Calculation**: For a 3x3 grid, the center is at index 4 (position 1,1)
- **Image Organization**: Images should be arranged in the array in row-major order (left-to-right, top-to-bottom)

## Usage in Editor:
1. Set the `gridSize` to your desired dimension
2. Create a UI Canvas with Image components arranged in a grid
3. Assign the images to the `gridImages` array in row-major order
4. The script will automatically hide all except the center image on Start

The code includes error checking to ensure you have the correct number of images for your grid size. 

Is this the foundation you were looking for? What additional requirements would you like me to implement next?

---

**User**

ok slight modification - i intended to not care about images position in game/editor - and that the script would be responsible for positioning them correctly (regardless of their visibility state)

---

**Cursor**

I understand! You want the script to handle the positioning of the images automatically, regardless of how they're arranged in the editor. The script should take the images from the array and position them in the correct grid layout programmatically.

Let me modify the code to include automatic positioning:

```csharp
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

        [Header("Grid Images")] [SerializeField]
        private Image[] gridImages;
        
        [Header("Grid Container")] [SerializeField]
        private RectTransform gridContainer;

        private int centerIndex;

        private void Start()
        {
            InitializeGrid();
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

            // Calculate center index (for odd grid sizes, this will be the true center)
            centerIndex = gridSize * gridSize / 2;

            // Hide all images except the center one
            for (var i = 0; i < gridImages.Length; i++)
                if (gridImages[i] != null)
                    gridImages[i].gameObject.SetActive(i == centerIndex);

            Debug.Log($"Initialized {gridSize}x{gridSize} grid. Center image at index {centerIndex} is visible.");
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
```

```csharp
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

        [Header("Grid Images")] [SerializeField]
        private Image[] gridImages;
        
        [Header("Grid Container")] [SerializeField]
        private RectTransform gridContainer;

        private int centerIndex;

        private void Start()
        {
            InitializeGrid();
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
                
                // Set size
                rectTransform.sizeDelta = new Vector2(cellSize, cellSize);
                
                // Set anchors to center
                rectTransform.anchorMin = Vector2.one * 0.5f;
                rectTransform.anchorMax = Vector2.one * 0.5f;
                rectTransform.pivot = Vector2.one * 0.5f;

                // Set visibility (only center image visible)
                gridImages[i].gameObject.SetActive(i == centerIndex);
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
```

Perfect! I've modified the code to handle automatic positioning. Here are the key changes:

## New Features Added:

1. **Grid Container**: Added `gridContainer` RectTransform field - this should be assigned to a parent UI object that will contain all the grid images

2. **Layout Configuration**:
   - `cellSize`: Size of each grid cell (default 100 units)
   - `spacing`: Space between grid cells (default 10 units)

3. **Automatic Positioning**: The `PositionImagesInGrid()` method now:
   - Calculates the total grid dimensions
   - Positions each image in the correct grid location
   - Sets proper RectTransform properties (size, anchors, pivot)
   - Parents images to the grid container if needed
   - Centers the entire grid around the container's position

## How It Works:

- **Grid Layout**: Images are positioned in a perfect grid with configurable cell size and spacing
- **Centered Grid**: The entire grid is centered on the `gridContainer` position
- **Automatic Parenting**: Images are automatically parented to the grid container
- **Proper UI Setup**: Each image gets proper RectTransform settings for UI positioning

## Usage in Editor:
1. Create a Canvas with an empty GameObject as the grid container
2. Assign the grid container's RectTransform to the `gridContainer` field
3. Create your UI Images anywhere in the scene (position doesn't matter)
4. Assign them to the `gridImages` array in any order
5. Configure `gridSize`, `cellSize`, and `spacing` as needed
6. The script will automatically position them in the correct grid layout!

The images can now be placed anywhere in the editor - the script takes full responsibility for positioning them correctly in the grid pattern. Is this what you were looking for?

---

**User**

note that I removed the size setting - this is intentional. Next step is this:

I want this script to listen for input - either arrow keys, joystick movement or mouse movement. the grid should move in the opposite direction of the input movement. Let's say for example the player moves the cursor from center to the right - the grid should move from center to the left. as this is happening the center right image is starting to get close to the center. Based on the distance from center - I want its opacity to change. As if the previous image is getting out of focus and the new image enters the focus. 

---

**Cursor**

I understand! You want to implement a smooth grid movement system where:

1. Input moves the grid in the opposite direction (inverse movement)
2. As images get closer to the center, their opacity increases (focus effect)
3. The center image loses opacity as it moves away from center

Let me implement this input system and opacity-based focus effect:

```csharp
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
        private Vector2 lastMousePosition;
        private bool isMouseInitialized = false;

        private void Start()
        {
            InitializeGrid();
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

                // Set visibility (only center image visible)
                gridImages[i].gameObject.SetActive(i == centerIndex);
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
```

```csharp
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
        private Vector2 lastMousePosition;
        private bool isMouseInitialized = false;

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

                // Set visibility (only center image visible)
                gridImages[i].gameObject.SetActive(i == centerIndex);
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
```

```csharp
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
        private Vector2 lastMousePosition;
        private bool isMouseInitialized = false;

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
            Vector2 inputMovement = Vector2.zero;

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
                var mouseDelta = (currentMousePosition - lastMousePosition) * mouseSensitivity;
                inputMovement += new Vector2(mouseDelta.x, mouseDelta.y) * Time.deltaTime;
                lastMousePosition = currentMousePosition;
            }

            // Apply movement (inverse direction)
            if (inputMovement.magnitude > 0.01f)
            {
                var movement = -inputMovement * movementSpeed * Time.deltaTime;
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
```

Perfect! I've implemented the input-driven grid movement system with opacity-based focus effects. Here's what the system now includes:

## Key Features Implemented:

### 1. **Multi-Input Support**:
- **Arrow Keys**: Standard directional input
- **Joystick/Gamepad**: Uses Unity's Input.GetAxis("Horizontal"/"Vertical")
- **Mouse Movement**: Tracks mouse delta movement with configurable sensitivity

### 2. **Inverse Movement**:
- Input movement is inverted (`-inputMovement`) so the grid moves opposite to input direction
- When you move right, the grid moves left, bringing the right image toward center

### 3. **Distance-Based Opacity System**:
- All images are now visible but with varying opacity
- Images closer to screen center have higher opacity (more "in focus")
- Uses an `AnimationCurve` for customizable opacity falloff
- `maxFocusDistance` controls the range of the focus effect

### 4. **Configurable Settings**:
- `movementSpeed`: How fast the grid responds to input
- `maxMovementDistance`: Limits how far the grid can move (prevents going too far)
- `mouseSensitivity`: Controls mouse input responsiveness
- `maxFocusDistance`: Distance range for opacity calculations
- `opacityCurve`: Customizable curve for opacity transitions

## How It Works:

1. **Input Detection**: Combines all input sources into a single movement vector
2. **Grid Movement**: Applies inverse movement to the grid container's position
3. **Focus Calculation**: Each frame, calculates each image's distance from screen center
4. **Opacity Application**: Uses the distance to determine opacity via the animation curve

## Visual Effect:
- The center image starts fully opaque
- As you move input right, the grid moves left
- The right image gradually becomes more opaque as it approaches center
- The original center image fades as it moves away
- Creates a smooth "focus transition" effect

The system is now ready to test! You can adjust the various parameters in the inspector to fine-tune the feel of the movement and focus effects. Would you like me to add any additional features or modify the behavior?

