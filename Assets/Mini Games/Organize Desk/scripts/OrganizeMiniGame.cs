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
        ItemPlacementSix,

        Trash
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

    public class OrganizeMiniGame : MiniGame
    {
        [Header("Organize Game Settings")] [SerializeField]
        private Transform itemsContainerTransform;

        [SerializeField] private OrganizableItem[] items;
        [SerializeField] private OrganizationPattern[] patterns;
        private int _organizedItemsCount;

        private GameObject _selectedItem;
        private GameObject _selectedPattern;
        private OrganizableItem _selectedSlot;

        private void Start()
        {
            StartMiniGame();

            ActivateRandomPattern();
            PlaceItemsRandomlyOnScreen();
            // Place3DItemsRandomlyOnScreen();
        }

        public void OnNotify(GameEventData gameEvent)
        {
            if (gameEvent.Name == GameEvents.CollisionEnter)
            {
                if (gameEvent.Data is not UIItemCollisionData data)
                    return;

                _organizedItemsCount++;
                return;
            }

            if (gameEvent.Name == GameEvents.CollisionExit)
            {
                if (gameEvent.Data is not UIItemCollisionData data)
                    return;

                _organizedItemsCount--;
                return;
            }

            if (gameEvent.Name == GameEvents.UIItemClicked)
            {
                if (gameEvent.Data is not UIItemClickData data)
                    return;

                _selectedItem = data.gameObject;
            }

            if (gameEvent.Name == GameEvents.UIItemPlacementClicked)
            {
                // Extract the gameObject from the anonymous object
                if (gameEvent.Data is not UIItemClickData data)
                    return;

                if (data.uiItemName == UIItem.Trash)
                {
                    if (_selectedItem != null)
                    {
                        _selectedItem.SetActive(false);
                        _selectedItem = null;
                    }

                    return;
                }

                var selectedPattern = data.gameObject;

                if (_selectedItem != null && selectedPattern != null)
                {
                    if (_organizedItemsCount == _selectedPattern.GetComponentsInChildren<UIClickNotifier>().Length)
                    {
                        CloseMiniGame();
                        return;
                    }

                    Instantiate(_selectedItem,
                        selectedPattern.transform.position,
                        Quaternion.identity,
                        itemsContainerTransform
                    );
                    _organizedItemsCount++;
                    _selectedItem.SetActive(false);
                    _selectedItem = null; // Reset selection after placement
                }
            }
        }

        private void Place3DItemsRandomlyOnScreen()
        {
            // Check if the itemsContainerTransform exists
            if (itemsContainerTransform == null)
            {
                Debug.LogError("itemsContainerTransform is not assigned!");
                return;
            }

            // Get the bounds of the container object
            // If the container has a collider, we can use its bounds
            var containerCollider = itemsContainerTransform.GetComponent<Collider>();
            var containerWidth = 10f;
            var containerHeight = 10f;
            var containerDepth = 10f;

            if (containerCollider != null)
            {
                var bounds = containerCollider.bounds;
                containerWidth = bounds.size.x;
                containerHeight = bounds.size.y;
                containerDepth = bounds.size.z;
            }
            else
            {
                // If no collider, use a default size or the transform's local scale
                containerWidth = itemsContainerTransform.localScale.x * 10f;
                containerHeight = itemsContainerTransform.localScale.y * 10f;
                containerDepth = itemsContainerTransform.localScale.z * 20f;
                Debug.LogWarning("No Collider found on container. Using estimated bounds based on transform scale.");
            }

            // Add padding to ensure objects aren't placed right at the edge
            var padding = 1f; // Adjust this value based on your 3D object sizes
            var minX = -containerWidth / 2 + padding;
            var maxX = containerWidth / 2 - padding;
            var minY = -containerHeight / 2 + padding;
            var maxY = containerHeight / 2 - padding;
            var minZ = -containerDepth / 2 + padding;
            var maxZ = containerDepth / 2 - padding;

            foreach (var item in items)
            {
                // Place items randomly within the calculated 3D bounds
                var randomX = Random.Range(minX, maxX);
                var randomY = Random.Range(minY, maxY);
                var randomZ = Random.Range(minZ, maxZ);
                var randomPosition = new Vector3(randomX, randomY, randomZ);

                // Instantiate the item as a 3D object
                var newItem = Instantiate(
                    item.itemObject,
                    randomPosition,
                    Quaternion.identity
                );
//                     // itemsContainerTransform.TransformPoint(randomPosition), // Convert local position to world position

                // Optional: Add random rotation
                // newItem.transform.rotation = Quaternion.Euler(
                //     Random.Range(0, 360),
                //     Random.Range(0, 360),
                //     Random.Range(0, 360)
                // );
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
            var padding = 0f; // Adjust this value based on your UI object sizes
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