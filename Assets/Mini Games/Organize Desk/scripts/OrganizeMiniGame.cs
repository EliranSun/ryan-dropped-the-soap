using System;
using System.Linq;
using UnityEngine;
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
        Pens,
        SoccerBall,

        // TODO: Refactor
        ItemPlacementOne,
        ItemPlacementTwo,
        ItemPlacementThree,
        ItemPlacementFour,
        ItemPlacementFive,
        ItemPlacementSix
    }

    [Serializable]
    public class OrganizationPattern
    {
        public GameObject pattern;
        public PatternName patternName;
        public UIItem uiItem;
    }

    [Serializable]
    public class OrganizableItem
    {
        public GameObject itemObject;
        public UIItem uiItem;
    }

    public class OrganizeMiniGame : MonoBehaviour
    {
        [SerializeField] private Transform itemsContainerTransform;
        [SerializeField] private OrganizableItem[] items;

        [SerializeField] private OrganizationPattern[] patterns;

        // private OrganizableItem _selectedItem;
        private GameObject _selectedItem;
        private GameObject _selectedPattern;
        private OrganizableItem _selectedSlot;

        private void Start()
        {
            ActivateRandomPattern();
            PlaceItemsRandomlyOnScreen();
        }

        // Update is called once per frame
        public void OnNotify(GameEventData gameEvent)
        {
            if (gameEvent.Name == GameEvents.UIItemClicked)
                // var itemName = (UIItem)gameEvent.Data;
                // Check if the clicked item exists in our items array
                // _selectedItem = items.First(item => item.uiItem == itemName);
                _selectedItem = (GameObject)gameEvent.Data;

            if (gameEvent.Name == GameEvents.UIItemPlacementClicked)
            {
                // var itemName = (UIItem)gameEvent.Data;
                // // Find the placement slot from the patterns array
                // var selectedPattern = Array.Find(
                //     _selectedPattern.GetComponentsInChildren<UIClickNotifier>(),
                //     pattern => pattern.uiItemName == itemName
                // );

                var selectedPattern = (GameObject)gameEvent.Data;

                if (_selectedItem != null && selectedPattern != null)
                {
                    // Use the pattern GameObject's position for placement
                    Instantiate(_selectedItem,
                        selectedPattern.transform.position,
                        Quaternion.identity,
                        itemsContainerTransform
                    );
                    _selectedItem.SetActive(false);
                    _selectedItem = null; // Reset selection after placement
                }
            }
        }

        private void PlaceItemsRandomlyOnScreen()
        {
            // Check if the itemsContainerTransform is a RectTransform (part of a Canvas)
            var containerRect = itemsContainerTransform as RectTransform;
            if (containerRect == null)
            {
                Debug.LogError("itemsContainerTransform is not a RectTransform! Make sure it's part of a Canvas.");
                return;
            }

            // Get the canvas rect dimensions
            var canvas = containerRect.GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("No Canvas found in parent hierarchy!");
                return;
            }

            // Get the container's rect dimensions
            var containerRect2D = containerRect.rect;
            var containerWidth = containerRect2D.width;
            var containerHeight = containerRect2D.height;

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

                // Set the anchored position on the RectTransform
                var itemRect = newItem.GetComponent<RectTransform>();
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
                            _selectedPattern = pattern.pattern;
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