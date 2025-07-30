using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string itemId;
    public string itemName;
    public GameObject itemPrefab;
    public Vector2 position;
    public string currentScene;
    public bool isCarriedByPlayer;

    public ItemData(string id, string name, GameObject prefab)
    {
        itemId = id;
        itemName = name;
        itemPrefab = prefab;
        position = Vector2.zero;
        currentScene = "";
        isCarriedByPlayer = false;
    }
}

[CreateAssetMenu(fileName = "InventoryManager", menuName = "Inventory/InventoryManager")]
public class InventoryManager : ScriptableObject
{
    [Header("Registered Items")]
    [SerializeField] private List<ItemData> allItems = new List<ItemData>();

    [Header("Runtime State - DO NOT EDIT")]
    [SerializeField] private List<ItemData> carriedItems = new List<ItemData>();
    [SerializeField] private List<ItemData> worldItems = new List<ItemData>();

    // Track spawned items to prevent duplicates
    private static Dictionary<string, GameObject> spawnedItemInstances = new Dictionary<string, GameObject>();

    #region Item Registration

    public void RegisterItem(string itemId, string itemName, GameObject prefab, Vector2 initialPosition, string sceneName)
    {
        var existingItem = allItems.FirstOrDefault(item => item.itemId == itemId);
        if (existingItem == null)
        {
            var newItem = new ItemData(itemId, itemName, prefab)
            {
                position = initialPosition,
                currentScene = sceneName
            };
            allItems.Add(newItem);

            // Only add to worldItems if not carried
            if (!carriedItems.Any(c => c.itemId == itemId))
            {
                worldItems.Add(newItem);
            }
        }
    }

    #endregion

    #region Item State Management

    public void PickupItem(string itemId)
    {
        var item = FindItem(itemId);
        if (item != null && !item.isCarriedByPlayer)
        {
            item.isCarriedByPlayer = true;
            worldItems.Remove(item);
            if (!carriedItems.Contains(item))
            {
                carriedItems.Add(item);
            }

            // Remove from spawned instances since it's now carried
            if (spawnedItemInstances.ContainsKey(itemId))
            {
                if (spawnedItemInstances[itemId] != null)
                {
                    DestroyImmediate(spawnedItemInstances[itemId]);
                }
                spawnedItemInstances.Remove(itemId);
            }

            Debug.Log($"Picked up: {item.itemName}");
        }
    }

    public void DropItem(string itemId, Vector2 position, string sceneName)
    {
        var item = carriedItems.FirstOrDefault(i => i.itemId == itemId);
        if (item != null)
        {
            item.isCarriedByPlayer = false;
            item.position = position;
            item.currentScene = sceneName;

            carriedItems.Remove(item);
            if (!worldItems.Contains(item))
            {
                worldItems.Add(item);
            }

            Debug.Log($"Dropped: {item.itemName} at {position} in {sceneName}");
        }
    }

    public List<ItemData> GetItemsInScene(string sceneName)
    {
        return worldItems.Where(item => item.currentScene == sceneName && !item.isCarriedByPlayer).ToList();
    }

    public List<ItemData> GetCarriedItems()
    {
        return new List<ItemData>(carriedItems);
    }

    public bool IsItemCarried(string itemId)
    {
        return carriedItems.Any(item => item.itemId == itemId);
    }

    public bool IsItemSpawned(string itemId)
    {
        return spawnedItemInstances.ContainsKey(itemId) && spawnedItemInstances[itemId] != null;
    }

    public void RegisterSpawnedItem(string itemId, GameObject spawnedObject)
    {
        if (spawnedItemInstances.ContainsKey(itemId))
        {
            // Destroy old instance if it exists
            if (spawnedItemInstances[itemId] != null)
            {
                DestroyImmediate(spawnedItemInstances[itemId]);
            }
        }
        spawnedItemInstances[itemId] = spawnedObject;
    }

    public void UnregisterSpawnedItem(string itemId)
    {
        if (spawnedItemInstances.ContainsKey(itemId))
        {
            spawnedItemInstances.Remove(itemId);
        }
    }

    #endregion

    #region Cleanup Methods

    public void CleanupScene(string sceneName)
    {
        var itemsToRemove = new List<string>();

        foreach (var kvp in spawnedItemInstances)
        {
            if (kvp.Value == null)
            {
                itemsToRemove.Add(kvp.Key);
            }
        }

        foreach (var itemId in itemsToRemove)
        {
            spawnedItemInstances.Remove(itemId);
        }

        Debug.Log($"Cleaned up {itemsToRemove.Count} null item references for scene: {sceneName}");
    }

    public void ClearAllSpawnedItems()
    {
        foreach (var kvp in spawnedItemInstances)
        {
            if (kvp.Value != null)
            {
                DestroyImmediate(kvp.Value);
            }
        }
        spawnedItemInstances.Clear();
        Debug.Log("Cleared all spawned item instances");
    }

    #endregion

    #region Utility

    private ItemData FindItem(string itemId)
    {
        return allItems.FirstOrDefault(item => item.itemId == itemId);
    }

    public void ResetInventory()
    {
        carriedItems.Clear();
        worldItems.Clear();
        worldItems.AddRange(allItems);

        // Reset all items to not carried
        foreach (var item in allItems)
        {
            item.isCarriedByPlayer = false;
        }

        ClearAllSpawnedItems();
        Debug.Log("Inventory reset");
    }

    #endregion

    #region Debug

    [ContextMenu("Log Inventory State")]
    public void LogInventoryState()
    {
        Debug.Log($"=== INVENTORY STATE ===");
        Debug.Log($"Total Items: {allItems.Count}");
        Debug.Log($"Carried Items ({carriedItems.Count}):");
        foreach (var item in carriedItems)
        {
            Debug.Log($"  - {item.itemName} ({item.itemId})");
        }

        Debug.Log($"World Items ({worldItems.Count}):");
        foreach (var item in worldItems)
        {
            Debug.Log($"  - {item.itemName} in {item.currentScene} at {item.position}");
        }

        Debug.Log($"Spawned Instances ({spawnedItemInstances.Count}):");
        foreach (var kvp in spawnedItemInstances)
        {
            Debug.Log($"  - {kvp.Key}: {(kvp.Value != null ? "Active" : "NULL")}");
        }
    }

    [ContextMenu("Force Cleanup")]
    public void ForceCleanup()
    {
        ClearAllSpawnedItems();
    }

    #endregion
}