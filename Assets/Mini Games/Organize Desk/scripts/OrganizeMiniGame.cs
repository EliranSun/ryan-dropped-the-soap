using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Mini_Games.Organize_Desk.scripts
{
    [Serializable]
    public enum PatternName
    {
        None,
        UShape,
        OShape
    }

    [Serializable]
    public enum UIItem
    {
        None,
        Notebook,
        Screen,
        SoccerBall
    }

    [Serializable]
    public class OrganizationPattern
    {
        public GameObject pattern;
        public PatternName patternName;
    }

    [Serializable]
    public class OrganizableItem
    {
        public GameObject itemObject;
        [FormerlySerializedAs("uiItems")] public UIItem uiItem;
    }

    public class OrganizeMiniGame : MonoBehaviour
    {
        [SerializeField] private Transform itemsContainerTransform;
        [SerializeField] private OrganizableItem[] items;
        [SerializeField] private OrganizationPattern[] patterns;
        private OrganizableItem _selectedItem;

        private void Start()
        {
            ActivateRandomPattern();
            PlaceItemsRandomlyOnScreen();
        }

        // Update is called once per frame
        public void OnNotify(GameEventData gameEvent)
        {
            if (gameEvent.Name == GameEvents.UIItemClicked)
            {
                var itemName = (UIItem)gameEvent.Data;

                // Check if the clicked item exists in our items array
                _selectedItem = items.First(item => item.uiItem == itemName);
            }
        }

        private void PlaceItemsRandomlyOnScreen()
        {
            // Check if the itemsContainerTransform is a RectTransform (part of a Canvas)
            RectTransform containerRect = itemsContainerTransform as RectTransform;
            if (containerRect == null)
            {
                Debug.LogError("itemsContainerTransform is not a RectTransform! Make sure it's part of a Canvas.");
                return;
            }

            // Get the canvas rect dimensions
            Canvas canvas = containerRect.GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("No Canvas found in parent hierarchy!");
                return;
            }

            // Get the container's rect dimensions
            Rect containerRect2D = containerRect.rect;
            float containerWidth = containerRect2D.width;
            float containerHeight = containerRect2D.height;

            // If the container has zero size, try to use the canvas size instead
            if (containerWidth <= 0 || containerHeight <= 0)
            {
                RectTransform canvasRect = canvas.GetComponent<RectTransform>();
                containerWidth = canvasRect.rect.width;
                containerHeight = canvasRect.rect.height;
            }

            // Add padding to ensure objects aren't placed right at the edge
            float padding = 50f; // Adjust this value based on your UI object sizes
            float minX = padding - containerWidth / 2;
            float maxX = containerWidth / 2 - padding;
            float minY = padding - containerHeight / 2;
            float maxY = containerHeight / 2 - padding;

            foreach (var item in items)
            {
                // Place items randomly within the calculated canvas bounds
                float randomX = Random.Range(minX, maxX);
                float randomY = Random.Range(minY, maxY);

                // Instantiate the item as a UI element
                GameObject newItem = Instantiate(
                    item.itemObject,
                    Vector3.zero, // Position will be set via RectTransform
                    Quaternion.identity,
                    itemsContainerTransform
                );

                // Set the anchored position on the RectTransform
                RectTransform itemRect = newItem.GetComponent<RectTransform>();
                if (itemRect != null)
                {
                    itemRect.anchoredPosition = new Vector2(randomX, randomY);
                }
                else
                {
                    Debug.LogWarning($"Item {item.uiItem} doesn't have a RectTransform component!");
                    // Fallback to setting the local position
                    newItem.transform.localPosition = new Vector3(randomX, randomY, 0);
                }
            }
        }

        private void ActivateRandomPattern()
        {
            // Deactivate all patterns first
            foreach (var pattern in patterns)
                if (pattern.pattern != null)
                    pattern.pattern.SetActive(false);

            // Get all pattern names except 'None'
            var patternNames = Enum.GetValues(typeof(PatternName)) as PatternName[];
            if (patternNames == null) return;
            {
                var validPatternNames =
                    patternNames
                        .Where(patternName => patternName != PatternName.None)
                        .ToList();

                // Select a random pattern name
                if (validPatternNames.Count > 0)
                {
                    var randomIndex = Random.Range(0, validPatternNames.Count);
                    var selectedPattern = validPatternNames[randomIndex];

                    Debug.Log($"Selected random pattern: {selectedPattern}");

                    // Find and activate the matching pattern
                    foreach (var pattern in patterns)
                        if (pattern.patternName == selectedPattern && pattern.pattern != null)
                        {
                            pattern.pattern.SetActive(true);
                            Debug.Log($"Activated pattern: {selectedPattern}");
                            break;
                        }
                }
                else
                {
                    Debug.LogWarning("No valid patterns found to activate.");
                }
            }
        }
    }
}