using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    /// <summary>
    /// Manages player inventory across scenes. Spawns carried items when scenes load.
    /// This version prevents item duplication by managing carried items centrally.
    /// </summary>
    public class PlayerInventoryController : MonoBehaviour
    {
        [Header("Inventory System")]
        [SerializeField] private InventoryManager inventoryManager;

        [Header("Carried Items Settings")]
        [SerializeField] private Transform carriedItemsParent;
        [SerializeField] private Vector2 defaultCarriedOffset = new Vector2(0f, 3f);
        [SerializeField] private LayerMask carriedItemLayer = -1;

        private List<InteractableItem> currentCarriedItems = new List<InteractableItem>();
        private bool isRestoringItems = false;

        public static PlayerInventoryController Instance { get; private set; }

        private void Awake()
        {
            // Singleton pattern - only one inventory controller should exist
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            // Use carriedItemsParent as this transform if not specified
            if (carriedItemsParent == null)
            {
                carriedItemsParent = transform;
            }
        }

        private void Start()
        {
            // Subscribe to scene change events
            SceneManager.sceneLoaded += OnSceneLoaded;

            // Restore carried items after scene load (with delay to ensure spawners have run)
            Invoke(nameof(RestoreCarriedItems), 0.2f);
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Clear current carried items list (they'll be destroyed with scene change)
            currentCarriedItems.Clear();

            // Re-attach carried items after scene change (with delay)
            Invoke(nameof(RestoreCarriedItems), 0.2f);
        }

        /// <summary>
        /// Restores all carried items from the inventory manager
        /// </summary>
        private void RestoreCarriedItems()
        {
            if (inventoryManager == null || isRestoringItems) return;

            isRestoringItems = true;
            var carriedItems = inventoryManager.GetCarriedItems();

            Debug.Log($"PlayerInventoryController: Attempting to restore {carriedItems.Count} carried items");

            foreach (var itemData in carriedItems)
            {
                RestoreCarriedItem(itemData);
            }

            isRestoringItems = false;
            Debug.Log($"PlayerInventoryController: Finished restoring carried items. Now have {currentCarriedItems.Count} active carried items");
        }

        /// <summary>
        /// Restores a specific carried item by spawning it and attaching to player
        /// </summary>
        private void RestoreCarriedItem(ItemData itemData)
        {
            // Check if item already exists in current carried items
            var existingItem = currentCarriedItems.Find(item => item.GetItemId() == itemData.itemId);
            if (existingItem != null)
            {
                Debug.Log($"Item {itemData.itemName} already restored, skipping");
                return;
            }

            // Check if the item already exists in the scene (maybe someone else spawned it)
            var existingInScene = FindObjectsOfType<InteractableItem>();
            foreach (var item in existingInScene)
            {
                if (item.GetItemId() == itemData.itemId && item.IsCarried())
                {
                    Debug.Log($"Item {itemData.itemName} already exists in scene as carried, adding to tracking");
                    currentCarriedItems.Add(item);
                    return;
                }
            }

            // Spawn the carried item
            if (itemData.itemPrefab != null)
            {
                GameObject carriedObject = Instantiate(itemData.itemPrefab, carriedItemsParent);
                carriedObject.layer = carriedItemLayer;

                var interactableItem = carriedObject.GetComponent<InteractableItem>();
                if (interactableItem != null)
                {
                    interactableItem.InitializeItem(itemData.itemId, itemData.itemName, inventoryManager);
                    currentCarriedItems.Add(interactableItem);

                    // Position the carried item
                    carriedObject.transform.localPosition = defaultCarriedOffset;

                    // Make sure it's visually set up as carried
                    var spriteRenderer = carriedObject.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.sortingOrder = 20; // High sorting order for carried items
                    }

                    Debug.Log($"Restored carried item: {itemData.itemName}");
                }
                else
                {
                    Debug.LogWarning($"Spawned carried item {itemData.itemName} doesn't have InteractableItem component!");
                    Destroy(carriedObject);
                }
            }
            else
            {
                Debug.LogWarning($"No prefab assigned for carried item: {itemData.itemName}");
            }
        }

        /// <summary>
        /// Registers an item as being picked up
        /// </summary>
        public void OnItemPickedUp(InteractableItem item)
        {
            if (!currentCarriedItems.Contains(item))
            {
                currentCarriedItems.Add(item);
            }

            Debug.Log($"Player picked up: {item.GetItemName()} (Total carried: {currentCarriedItems.Count})");
        }

        /// <summary>
        /// Registers an item as being dropped
        /// </summary>
        public void OnItemDropped(InteractableItem item)
        {
            currentCarriedItems.Remove(item);
            Debug.Log($"Player dropped: {item.GetItemName()} (Total carried: {currentCarriedItems.Count})");
        }

        /// <summary>
        /// Gets all currently carried items
        /// </summary>
        public List<InteractableItem> GetCarriedItems()
        {
            // Clean up any null references
            currentCarriedItems.RemoveAll(item => item == null);
            return new List<InteractableItem>(currentCarriedItems);
        }

        /// <summary>
        /// Checks if player is carrying any items
        /// </summary>
        public bool HasCarriedItems()
        {
            // Clean up any null references first
            currentCarriedItems.RemoveAll(item => item == null);
            return currentCarriedItems.Count > 0;
        }

        /// <summary>
        /// Gets the number of carried items
        /// </summary>
        public int GetCarriedItemCount()
        {
            // Clean up any null references first
            currentCarriedItems.RemoveAll(item => item == null);
            return currentCarriedItems.Count;
        }

        /// <summary>
        /// Finds a carried item by ID
        /// </summary>
        public InteractableItem FindCarriedItem(string itemId)
        {
            return currentCarriedItems.Find(item => item != null && item.GetItemId() == itemId);
        }

        /// <summary>
        /// Sets the inventory manager (useful for runtime setup)
        /// </summary>
        public void SetInventoryManager(InventoryManager manager)
        {
            inventoryManager = manager;
        }

        #region Debug Utilities

        [ContextMenu("List Carried Items")]
        public void ListCarriedItems()
        {
            currentCarriedItems.RemoveAll(item => item == null);

            Debug.Log($"=== CARRIED ITEMS ({currentCarriedItems.Count}) ===");

            if (currentCarriedItems.Count == 0)
            {
                Debug.Log("Player is not carrying any items");
                return;
            }

            foreach (var item in currentCarriedItems)
            {
                Debug.Log($"- {item.GetItemName()} ({item.GetItemId()})");
            }

            // Also check what the inventory manager thinks is carried
            if (inventoryManager != null)
            {
                var managerCarriedItems = inventoryManager.GetCarriedItems();
                Debug.Log($"=== INVENTORY MANAGER THINKS IS CARRIED ({managerCarriedItems.Count}) ===");
                foreach (var item in managerCarriedItems)
                {
                    Debug.Log($"- {item.itemName} ({item.itemId})");
                }
            }
        }

        [ContextMenu("Force Restore Carried Items")]
        public void ForceRestoreCarriedItems()
        {
            currentCarriedItems.Clear();
            RestoreCarriedItems();
        }

        [ContextMenu("Clear Carried Items")]
        public void ClearCarriedItems()
        {
            foreach (var item in currentCarriedItems)
            {
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }
            currentCarriedItems.Clear();
            Debug.Log("Cleared all carried items");
        }

        #endregion
    }
}