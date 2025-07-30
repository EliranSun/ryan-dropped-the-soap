using UnityEngine;

/// <summary>
/// Helper script to set up the inventory system in your game.
/// This script provides utilities and guidance for implementing the cross-scene inventory system.
/// </summary>
public class InventorySystemSetup : MonoBehaviour
{
    [Header("Setup Wizard")]
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private GameObject[] itemPrefabs;
    [SerializeField] private string[] itemNames;
    [SerializeField] private string[] itemIds;

    [Header("Demo Setup")]
    [SerializeField] private bool createDemoItems = false;
    [SerializeField] private Vector2[] demoItemPositions;

    private void Start()
    {
        if (createDemoItems)
        {
            CreateDemoItems();
        }
    }

    /// <summary>
    /// Creates demo items for testing the inventory system
    /// </summary>
    [ContextMenu("Create Demo Items")]
    public void CreateDemoItems()
    {
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager is not assigned!");
            return;
        }

        if (itemPrefabs == null || itemPrefabs.Length == 0)
        {
            Debug.LogWarning("No item prefabs assigned for demo creation");
            return;
        }

        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        for (int i = 0; i < itemPrefabs.Length; i++)
        {
            string itemName = i < itemNames.Length ? itemNames[i] : $"Item_{i}";
            string itemId = i < itemIds.Length ? itemIds[i] : $"item_{i}_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
            Vector2 position = i < demoItemPositions.Length ? demoItemPositions[i] : Vector2.zero;

            inventoryManager.RegisterItem(itemId, itemName, itemPrefabs[i], position, currentScene);
            Debug.Log($"Registered demo item: {itemName} ({itemId})");
        }

        Debug.Log($"Created {itemPrefabs.Length} demo items in scene: {currentScene}");
    }

    /// <summary>
    /// Validates the setup of the inventory system
    /// </summary>
    [ContextMenu("Validate Setup")]
    public void ValidateSetup()
    {
        Debug.Log("=== INVENTORY SYSTEM VALIDATION ===");

        // Check InventoryManager
        if (inventoryManager == null)
        {
            Debug.LogError("❌ InventoryManager is not assigned!");
        }
        else
        {
            Debug.Log("✅ InventoryManager is assigned");
        }

        // Check PlayerStatesController
        if (Player.PlayerStatesController.Instance == null)
        {
            Debug.LogWarning("⚠️ PlayerStatesController.Instance is null - make sure it's in the scene");
        }
        else
        {
            Debug.Log("✅ PlayerStatesController.Instance found");
        }

        // Check PlayerInventoryController
        if (Player.PlayerInventoryController.Instance == null)
        {
            Debug.LogWarning("⚠️ PlayerInventoryController.Instance is null - add it to the player");
        }
        else
        {
            Debug.Log("✅ PlayerInventoryController.Instance found");
        }

        // Check SceneItemSpawner
        var spawner = FindObjectOfType<SceneItemSpawner>();
        if (spawner == null)
        {
            Debug.LogWarning("⚠️ No SceneItemSpawner found in scene - add one to spawn items");
        }
        else
        {
            Debug.Log("✅ SceneItemSpawner found in scene");
        }

        Debug.Log("=== VALIDATION COMPLETE ===");
    }

    /// <summary>
    /// Creates the basic setup for the inventory system
    /// </summary>
    [ContextMenu("Auto Setup")]
    public void AutoSetup()
    {
        Debug.Log("Setting up inventory system...");

        // Create InventoryManager asset if it doesn't exist
        if (inventoryManager == null)
        {
            Debug.Log("Creating InventoryManager asset...");
            // Note: This would need to be done manually or through an editor script
            Debug.LogWarning("Please create an InventoryManager asset: Right-click in Project → Create → Inventory → InventoryManager");
        }

        // Check if SceneItemSpawner exists
        var spawner = FindObjectOfType<SceneItemSpawner>();
        if (spawner == null)
        {
            Debug.Log("Adding SceneItemSpawner to scene...");
            var spawnerGO = new GameObject("Scene Item Spawner");
            spawner = spawnerGO.AddComponent<SceneItemSpawner>();
        }

        // Check PlayerInventoryController on player
        if (Player.PlayerStatesController.Instance != null)
        {
            var inventoryController = Player.PlayerStatesController.Instance.GetComponent<Player.PlayerInventoryController>();
            if (inventoryController == null)
            {
                Debug.Log("Adding PlayerInventoryController to player...");
                Player.PlayerStatesController.Instance.gameObject.AddComponent<Player.PlayerInventoryController>();
            }
        }

        Debug.Log("Auto setup complete! Please validate setup.");
    }
}