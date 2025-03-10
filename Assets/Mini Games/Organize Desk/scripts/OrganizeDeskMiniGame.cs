using UnityEngine;

namespace Mini_Games.Organize_Desk.scripts
{
    public class OrganizeDeskMiniGame : MiniGame
    {
        [SerializeField] private GameObject[] items;
        [SerializeField] private GameObject itemsContainer;

        private void PlaceItemsRandomlyOnScreen()
        {
            // Check if the itemsContainerTransform is a RectTransform (part of a Canvas)
            if (itemsContainer.transform == null)
            {
                Debug.LogError("itemsContainerTransform is not a RectTransform! Make sure it's part of a Canvas.");
                return;
            }

            // Get the container's rect dimensions
            var containerRect2D = containerRect.rect;
            var containerWidth = containerRect2D.width;
            var containerHeight = containerRect2D.height;

            // Calculate an appropriate depth based on the container's width or height
            // This creates a proportional depth relative to the container size
            var containerDepth = Mathf.Max(containerWidth, containerHeight) * 0.1f;

            // Alternative: You could also use the scale of the container
            // var containerDepth = containerRect.lossyScale.z * 10f;

            // If the container has zero size, try to use the canvas size instead
            if (containerWidth <= 0 || containerHeight <= 0)
            {
                var canvasRect = canvas.GetComponent<RectTransform>();
                containerWidth = canvasRect.rect.width;
                containerHeight = canvasRect.rect.height;
            }

            // Add padding to ensure objects aren't placed right at the edge
            var padding = 50f; // Adjust this value based on your UI object sizes
            var minX = padding - containerWidth / 2;
            var maxX = containerWidth / 2 - padding;
            var minY = padding - containerHeight / 2;
            var maxY = containerHeight / 2 - padding;

            foreach (var item in items)
            {
                // Place items randomly within the calculated canvas bounds
                var randomX = Random.Range(minX, maxX);
                var randomY = Random.Range(minY, maxY);

                // Instantiate the item as a UI element
                var newItem = Instantiate(
                    item.itemObject,
                    Vector3.zero, // Position will be set via RectTransform
                    Quaternion.identity,
                    itemsContainerTransform
                );

                newItem.GetComponent<UIClickNotifier>().SetUIItemData(
                    item.uiItem,
                    GameEvents.UIItemClicked,
                    OnNotify
                );

                var itemRect = newItem.GetComponent<Transform>();
                if (itemRect != null)
                    itemRect.position = new Vector2(randomX, randomY);
                else
                    newItem.transform.localPosition = new Vector2(randomX, randomY);
            }
        }
    }
}