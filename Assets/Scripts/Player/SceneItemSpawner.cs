using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Spawns items in scenes based on InventoryManager data.
/// Enhanced version with better duplicate prevention.
/// </summary>
public class SceneItemSpawner : MonoBehaviour
{
    [Header("Inventory System")]
    [SerializeField] private InventoryManager inventoryManager;

    [Header("Spawn Settings")]
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private bool cleanupOnAwake = true;
    [SerializeField] private Transform itemParent; // Optional parent for organization

    private string currentSceneName;

    private void Awake()
    {
        currentSceneName = SceneManager.GetActiveScene().name;

        if (cleanupOnAwake && inventoryManager != null)
        {
            // Clean up any leftover references from previous scene loads
            inventoryManager.CleanupScene(currentSceneName);
        }
    }

    private void Start()
    {
        if (spawnOnStart)
        {
            // Small delay to ensure all systems are initialized
            Invoke(nameof(SpawnSceneItems), 0.1f);
        }
    }

    /// <summary>
    /// Spawns all items that should be in the current scene
    /// </summary>
    public void SpawnSceneItems()
    {
        if (inventoryManager == null)
        {
            Debug.LogWarning("SceneItemSpawner: No InventoryManager assigned!");
            return;
        }

        var itemsInScene = inventoryManager.GetItemsInScene(currentSceneName);
        int spawnedCount = 0;

        foreach (var itemData in itemsInScene)
        {
            if (TrySpawnItem(itemData))
            {
                spawnedCount++;
            }
        }

        Debug.Log($"SceneItemSpawner: Spawned {spawnedCount}/{itemsInScene.Count} items in scene: {currentSceneName}");
    }

    /// <summary>
    /// Attempts to spawn a specific item, with comprehensive duplicate checking
    /// </summary>
    private bool TrySpawnItem(ItemData itemData)
    {
        if (itemData.itemPrefab == null)
        {
            Debug.LogWarning($"No prefab assigned for item: {itemData.itemName}");
            return false;
        }

        // Skip carried items
        if (itemData.isCarriedByPlayer)
        {
            Debug.Log($"Skipping carried item: {itemData.itemName}");
            return false;
        }

        // Check if already spawned in inventory manager
        if (inventoryManager.IsItemSpawned(itemData.itemId))
        {
            Debug.Log($"Item {itemData.itemName} already tracked as spawned, skipping");
            return false;
        }

        // Check for existing items in scene with same ID
        var existingItem = FindExistingItemInScene(itemData.itemId);
        if (existingItem != null)
        {
            Debug.Log($"Item {itemData.itemName} already exists in scene, registering existing instance");
            inventoryManager.RegisterSpawnedItem(itemData.itemId, existingItem);
            return false; // Don't count as "spawned" since it already existed
        }

        // Safe to spawn
        return SpawnItem(itemData);
    }

    /// <summary>
    /// Actually spawns the item
    /// </summary>
    private bool SpawnItem(ItemData itemData)
    {
        Vector3 spawnPosition = new Vector3(itemData.position.x, itemData.position.y, 0);
        GameObject spawnedItem = Instantiate(itemData.itemPrefab, spawnPosition, Quaternion.identity);

        // Set parent if specified
        if (itemParent != null)
        {
            spawnedItem.transform.SetParent(itemParent);
        }

        // Initialize the InteractableItem component
        var interactableItem = spawnedItem.GetComponent<InteractableItem>();
        if (interactableItem != null)
        {
            interactableItem.InitializeItem(itemData.itemId, itemData.itemName, inventoryManager);
        }
        else
        {
            Debug.LogWarning($"Spawned item {itemData.itemName} doesn't have InteractableItem component!");
            // Still register it to prevent re-spawning
            inventoryManager.RegisterSpawnedItem(itemData.itemId, spawnedItem);
        }

        return true;
    }

    /// <summary>
    /// Finds an existing item in the scene by ID
    /// </summary>
    private GameObject FindExistingItemInScene(string itemId)
    {
        var existingItems = FindObjectsOfType<InteractableItem>();
        foreach (var item in existingItems)
        {
            if (item.GetItemId() == itemId)
            {
                return item.gameObject;
            }
        }
        return null;
    }

    /// <summary>
    /// Clears all items from the current scene
    /// </summary>
    public void ClearSceneItems()
    {
        var itemsInScene = FindObjectsOfType<InteractableItem>();
        int clearedCount = 0;

        foreach (var item in itemsInScene)
        {
            if (!item.IsCarried()) // Don't destroy carried items
            {
                Destroy(item.gameObject);
                clearedCount++;
            }
        }

        if (inventoryManager != null)
        {
            inventoryManager.CleanupScene(currentSceneName);
        }

        Debug.Log($"Cleared {clearedCount} items from scene: {currentSceneName}");
    }

    /// <summary>
    /// Manually refresh items in scene (useful for debugging)
    /// </summary>
    [ContextMenu("Refresh Scene Items")]
    public void RefreshSceneItems()
    {
        ClearSceneItems();
        // Wait a frame for cleanup to complete
        Invoke(nameof(SpawnSceneItems), 0.1f);
    }

    #region Debug Utilities

    [ContextMenu("List Items in Scene")]
    public void ListItemsInScene()
    {
        if (inventoryManager == null) return;

        var itemsInScene = inventoryManager.GetItemsInScene(currentSceneName);
        Debug.Log($"=== ITEMS THAT SHOULD BE IN SCENE: {currentSceneName} ===");

        if (itemsInScene.Count == 0)
        {
            Debug.Log("No items should be in this scene");
        }
        else
        {
            foreach (var item in itemsInScene)
            {
                Debug.Log($"- {item.itemName} ({item.itemId}) at {item.position} | Carried: {item.isCarriedByPlayer}");
            }
        }

        // Also list what's actually in the scene
        var actualItems = FindObjectsOfType<InteractableItem>();
        Debug.Log($"=== ITEMS ACTUALLY IN SCENE: {currentSceneName} ===");
        if (actualItems.Length == 0)
        {
            Debug.Log("No items actually found in scene");
        }
        else
        {
            foreach (var item in actualItems)
            {
                Debug.Log($"- {item.GetItemName()} ({item.GetItemId()}) | Carried: {item.IsCarried()}");
            }
        }
    }

    [ContextMenu("Force Cleanup Scene")]
    public void ForceCleanupScene()
    {
        if (inventoryManager != null)
        {
            inventoryManager.CleanupScene(currentSceneName);
            inventoryManager.ClearAllSpawnedItems();
        }
        ClearSceneItems();
    }

    #endregion
}