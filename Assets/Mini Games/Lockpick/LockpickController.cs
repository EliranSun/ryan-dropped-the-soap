using UnityEngine;
using UnityEngine.UI;

namespace Mini_Games.Lockpick
{
    public class LockpickController : MonoBehaviour
    {
        [Header("Grid Configuration")] [SerializeField]
        private int gridSize = 3;

        [Header("Grid Images")] [SerializeField]
        private Image[] gridImages;

        private void Update()
        {
            UpdateImageOpacity();
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