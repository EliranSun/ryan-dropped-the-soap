using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mini_Games.Organize_Desk.scripts
{
    public enum DialogLineType
    {
        Good,
        Bad
    }

    public class OrganizeDeskMiniGame : MiniGame
    {
        [Header("Organize Desk Settings")] [SerializeField]
        private GameObject[] items;

        [SerializeField] private GameObject itemsContainer;
        [SerializeField] private float yOffset = 2f;

        private readonly List<GameObject> _itemsRef = new();
        private readonly HashSet<int> _nonEssentialItemIds = new();

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
            }
        }

        public override void OnNotify(GameEventData gameEvent)
        {
            base.OnNotify(gameEvent);

            if (gameEvent.Name == GameEvents.MiniGameIndicationTrigger)
                isGameActive = (MiniGameName)gameEvent.Data == MiniGameName.Organize;

            if (gameEvent.Name == GameEvents.DeskItemsChanged)
            {
                var gameObjectRef = gameEvent.Data as GameObject;
                if (!gameObjectRef)
                    return;

                var item = gameObjectRef.GetComponent<ScoreableItem>();
                var isNonEssentialItem = item.score < 0;

                if (!isNonEssentialItem)
                    return;

                if (item.offTable)
                {
                    print($"Removing non-essential {gameObjectRef.name} id: {gameObjectRef.GetInstanceID()}");
                    _nonEssentialItemIds.Remove(gameObjectRef.GetInstanceID());
                }
                else
                {
                    print($"Adding non-essential {gameObjectRef.name} id: {gameObjectRef.GetInstanceID()}");
                    _nonEssentialItemIds.Add(gameObjectRef.GetInstanceID());
                }
            }
        }

        protected override void StartMiniGame()
        {
            if (!isGameActive)
                return;

            base.StartMiniGame();
            PlaceItemsRandomlyOnScreen();
        }

        protected override void CloseMiniGame(bool isGameWon = false)
        {
            var gameWon = _nonEssentialItemIds.Count == 0;
            base.CloseMiniGame(gameWon);
        }
    }
}