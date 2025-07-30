using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class InteractableItem : ObserverSubject
{
    [Header("Item Configuration")]
    [SerializeField] private string itemId;
    [SerializeField] private string itemName;
    [SerializeField] private InventoryManager inventoryManager;

    [Header("Visual Settings")]
    [SerializeField] private int carriedSortingOrder = 20;
    [SerializeField] private int droppedSortingOrder = 4;
    [SerializeField] private Vector2 carriedOffset = new Vector2(0f, 3f);

    [Header("Game Events")]
    [SerializeField] private GameEvents holdGameEvent = GameEvents.PickedItem;
    [SerializeField] private GameEvents releaseGameEvent = GameEvents.DroppedItem;

    private Collider2D _collider2D;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody2D;
    private Transform _playerTransform;
    private bool _isCarried = false;
    private bool _isInitialized = false;

    private void Awake()
    {
        _collider2D = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody2D = GetComponent<Rigidbody2D>();

        // Generate unique ID if not set
        if (string.IsNullOrEmpty(itemId))
        {
            itemId = $"{itemName}_{GetInstanceID()}";
        }
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (_isInitialized) return;

        // Get player reference
        if (Player.PlayerStatesController.Instance != null)
        {
            _playerTransform = Player.PlayerStatesController.Instance.transform;
        }

        // Register with inventory manager
        if (inventoryManager != null)
        {
            var currentScene = SceneManager.GetActiveScene().name;

            // Only register if this is a "fresh" spawn, not a carried item
            if (!inventoryManager.IsItemCarried(itemId))
            {
                inventoryManager.RegisterItem(itemId, itemName, gameObject, transform.position, currentScene);
            }

            // Register this spawned instance to prevent duplicates
            inventoryManager.RegisterSpawnedItem(itemId, gameObject);

            // Check if this item should be carried (in case of scene reload)
            if (inventoryManager.IsItemCarried(itemId))
            {
                AttachToPlayer();
            }
        }

        _isInitialized = true;
    }

    private void OnMouseDown()
    {
        if (!_isInitialized) Initialize();

        if (_isCarried)
        {
            DropItem();
        }
        else
        {
            PickupItem();
        }
    }

    private void PickupItem()
    {
        if (_playerTransform == null || inventoryManager == null) return;

        // Update inventory manager
        inventoryManager.PickupItem(itemId);

        AttachToPlayer();

        // Notify game events
        Notify(holdGameEvent, transform.position);
    }

    private void DropItem()
    {
        if (inventoryManager == null) return;

        var currentScene = SceneManager.GetActiveScene().name;
        inventoryManager.DropItem(itemId, transform.position, currentScene);

        DetachFromPlayer();

        // Re-register as spawned item in new location
        inventoryManager.RegisterSpawnedItem(itemId, gameObject);

        // Notify game events
        Notify(releaseGameEvent, transform.position);
    }

    private void AttachToPlayer()
    {
        if (_playerTransform == null) return;

        _isCarried = true;

        // Physics settings for carried state
        if (_rigidbody2D != null)
        {
            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        }
        _collider2D.isTrigger = true;

        // Visual settings
        _spriteRenderer.sortingOrder = carriedSortingOrder;

        // Parent to player and position
        transform.SetParent(_playerTransform);
        transform.localPosition = carriedOffset;
    }

    private void DetachFromPlayer()
    {
        _isCarried = false;

        // Physics settings for dropped state
        if (_rigidbody2D != null)
        {
            _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        }
        _collider2D.isTrigger = false;

        // Visual settings
        _spriteRenderer.sortingOrder = droppedSortingOrder;

        // Unparent from player
        transform.SetParent(null);
    }

    // Called by SceneItemSpawner when spawning items
    public void InitializeItem(string id, string name, InventoryManager manager)
    {
        itemId = id;
        itemName = name;
        inventoryManager = manager;
        _isInitialized = false; // Force re-initialization
        Initialize();
    }

    // Utility methods
    public string GetItemId() => itemId;
    public string GetItemName() => itemName;
    public bool IsCarried() => _isCarried;

    private void OnDestroy()
    {
        // Unregister from inventory manager when destroyed
        if (inventoryManager != null)
        {
            inventoryManager.UnregisterSpawnedItem(itemId);
        }
    }

    #region Editor Helpers

    [ContextMenu("Set Unique ID")]
    private void SetUniqueId()
    {
        itemId = $"{itemName}_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
    }

    [ContextMenu("Force Initialize")]
    private void ForceInitialize()
    {
        _isInitialized = false;
        Initialize();
    }

    #endregion
}