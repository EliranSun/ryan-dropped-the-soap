using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mini_Games.Organize_Desk.scripts
{
    public class OrganizeDeskMiniGame : MiniGame
    {
        [SerializeField] private GameObject[] items;
        [SerializeField] private GameObject itemsContainer;
        [SerializeField] private float yOffset = 2f;

        private readonly List<GameObject> _itemsRef = new();
        private readonly HashSet<GameObject> _nonEssentialItems = new();

        private void Start()
        {
            StartMiniGame();
            PlaceItemsRandomlyOnScreen();
        }

        private void PlaceItemsRandomlyOnScreen()
        {
            // Check if the itemsContainer has a transform
            if (itemsContainer == null || itemsContainer.transform == null)
            {
                Debug.LogError("itemsContainer is null or has no transform!");
                return;
            }

            // Get the container's bounds using its renderer or collider
            Bounds containerBounds;

            // Try to get bounds from a renderer
            var renderer = itemsContainer.GetComponent<Renderer>();
            if (renderer != null)
            {
                containerBounds = renderer.bounds;
            }
            // If no renderer, try to get bounds from a collider
            else
            {
                var collider = itemsContainer.GetComponent<Collider>();
                if (collider != null)
                {
                    containerBounds = collider.bounds;
                }
                else
                {
                    // If no renderer or collider, use a default size based on the transform's scale
                    var scale = itemsContainer.transform.lossyScale;
                    containerBounds = new Bounds(
                        itemsContainer.transform.position,
                        new Vector3(scale.x * 10f, scale.y * 10f, scale.z * 2f)
                    );
                    Debug.LogWarning(
                        "No Renderer or Collider found on itemsContainer. Using default bounds based on transform scale.");
                }
            }

            // Add padding to ensure objects aren't placed right at the edge
            var padding = 0.5f; // Adjust this value based on your object sizes
            var minX = containerBounds.min.x + padding;
            var maxX = containerBounds.max.x - padding;
            var minY = containerBounds.min.y + padding;
            var maxY = containerBounds.max.y - padding;
            var z = itemsContainer.transform.position.z;

            foreach (var item in items)
            {
                // Place items randomly within the calculated container bounds
                var randomX = Random.Range(minX, maxX);
                var randomY = Random.Range(minY, maxY);

                // Instantiate the item as a world space object
                var newItem = Instantiate(
                    item,
                    new Vector3(randomX, randomY + yOffset, z),
                    Quaternion.identity,
                    itemsContainer.transform
                );

                _itemsRef.Add(newItem);

                // newItem.GetComponent<UIClickNotifier>().SetUIItemData(
                //     item,
                //     GameEvents.UIItemClicked,
                //     OnNotify
                // );

                // You can add any additional setup for the item here
                // For example, if you have a component that handles item interaction:
                // var itemComponent = newItem.GetComponent<ItemComponent>();
                // if (itemComponent != null)
                // {
                //     itemComponent.Initialize();
                // }
            }
        }

        public void OnNotify(GameEventData gameEvent)
        {
            if (gameEvent.Name == GameEvents.DeskItemsChanged)
            {
                var gameObjectName = gameEvent.Data as GameObject;
                foreach (var itemRef in _itemsRef)
                {
                    var item = itemRef.GetComponent<ScoreableItem>();
                    var isEssentialItem = item.score > 0;
                    if (item == null || isEssentialItem) continue;

                    if (item.isThrown && item.offTable)
                        _nonEssentialItems.Remove(gameObjectName);
                    else
                        _nonEssentialItems.Add(gameObjectName);
                }

                Debug.Log(string.Join(", ", _nonEssentialItems.Select(item => item.name)));
            }
        }
    }
}