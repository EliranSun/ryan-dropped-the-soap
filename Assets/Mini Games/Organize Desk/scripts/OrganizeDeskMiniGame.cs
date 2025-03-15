using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Dialog.Scripts;
using System;
namespace Mini_Games.Organize_Desk.scripts
{
    public enum DialogLineType
    {
        Good,
        Bad
    }

    [Serializable]
    public class TypedDialogLine
    {
        public NarrationDialogLine dialogLine;
        public DialogLineType type;

        public TypedDialogLine(NarrationDialogLine line, DialogLineType lineType)
        {
            dialogLine = line;
            type = lineType;
        }
    }

    public class DeskDialogLines : ScriptableObject
    {
        [SerializeField] private TypedDialogLine[] dialogLines;

        public NarrationDialogLine GetRandomLine(DialogLineType type)
        {
            // Filter lines by type
            var filteredLines = Array.FindAll(dialogLines, line => line.type == type);

            // If no lines of this type, return null
            if (filteredLines.Length == 0)
                return null;

            // Return a random line of the specified type
            var randomIndex = UnityEngine.Random.Range(0, filteredLines.Length);
            return filteredLines[randomIndex].dialogLine;
        }
    }

    public class OrganizeDeskMiniGame : MiniGame
    {
        [SerializeField] private GameObject[] items;
        [SerializeField] private GameObject itemsContainer;
        [SerializeField] private float yOffset = 2f;
        [SerializeField] private DeskDialogLines deskDialogLines;

        private readonly List<GameObject> _itemsRef = new();
        private readonly HashSet<GameObject> _nonEssentialItems = new();

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

        public override void OnNotify(GameEventData gameEvent)
        {
            base.OnNotify(gameEvent);

            if (gameEvent.Name == GameEvents.DeskItemsChanged)
            {
                var gameObjectName = gameEvent.Data as GameObject;
                foreach (var itemRef in _itemsRef)
                {
                    var item = itemRef.GetComponent<ScoreableItem>();
                    var isEssentialItem = item.score > 0;
                    if (item == null || isEssentialItem) continue;

                    if (item.offTable)
                        _nonEssentialItems.Remove(gameObjectName);
                    else
                        _nonEssentialItems.Add(gameObjectName);
                }

                // Debug.Log(string.Join(", ", _nonEssentialItems.Select(item => item.name)));
            }
        }

        protected override void StartMiniGame()
        {
            base.StartMiniGame();
            PlaceItemsRandomlyOnScreen();
        }

        protected override void CloseMiniGame()
        {
            base.CloseMiniGame();

            bool gameWon = _nonEssentialItems.Count == 0;

            // Get a random dialog line based on game outcome
            NarrationDialogLine dialogLine = null;
            if (deskDialogLines != null)
            {
                dialogLine = deskDialogLines.GetRandomLine(
                    gameWon ? DialogLineType.Good : DialogLineType.Bad
                );
            }

            // Trigger the dialog line if one was found
            if (dialogLine != null)
            {
                Notify(GameEvents.TriggerSpecificDialogLine, dialogLine);
            }

            // Notify about game outcome
            Notify(gameWon ? GameEvents.MiniGameWon : GameEvents.MiniGameLost);
        }
    }
}