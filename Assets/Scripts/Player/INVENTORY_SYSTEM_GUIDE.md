# Cross-Scene Inventory System - Implementation Guide

## Overview

This inventory system allows players to pick up items in one scene, carry them around, and drop them in different scenes. The system is built around ScriptableObjects for persistent data storage and integrates with your existing PlayerStatesController and event system.

## Core Components

### 1. **InventoryManager** (ScriptableObject)
- **Purpose**: Central data storage for all inventory state
- **Location**: `Assets/Scripts/Common/scripts/InventoryManager.cs`
- **Features**:
  - Tracks all items in the game
  - Manages carried vs. world items
  - Handles item state persistence
  - Scene-based item organization

### 2. **InteractableItem** (MonoBehaviour)
- **Purpose**: Individual item behavior and interaction
- **Location**: `Assets/Scripts/Common/scripts/InteractableItem.cs`
- **Features**:
  - Click to pick up/drop items
  - Automatic scene persistence
  - Visual feedback (sorting order, positioning)
  - Integration with event system

### 3. **SceneItemSpawner** (MonoBehaviour)
- **Purpose**: Spawns items in scenes based on inventory data
- **Location**: `Assets/Scripts/Common/scripts/SceneItemSpawner.cs`
- **Features**:
  - Automatic item spawning on scene load
  - Prevents item duplication
  - Scene-specific item management

### 4. **PlayerInventoryController** (MonoBehaviour)
- **Purpose**: Manages carried items on the player
- **Location**: `Assets/Scripts/Player/PlayerInventoryController.cs`
- **Features**:
  - Handles carried items across scenes
  - Integrates with PlayerStatesController
  - Visual management of carried items

## Setup Instructions

### Step 1: Create InventoryManager Asset
1. Right-click in your Project window
2. Go to `Create → Inventory → InventoryManager`
3. Name it "GameInventoryManager" or similar
4. Place it in your `Assets/Data/` folder

### Step 2: Set Up Player
1. Add `PlayerInventoryController` component to your player GameObject
2. Assign the InventoryManager asset to the controller
3. Make sure your PlayerStatesController has DontDestroyOnLoad (already implemented)

### Step 3: Set Up Scenes
1. In each scene where items can appear, add a `SceneItemSpawner` component
2. Assign the same InventoryManager asset to each spawner
3. Optionally create an empty GameObject as "Items Parent" for organization

### Step 4: Create Item Prefabs
1. Create prefabs for your pickable items
2. Add required components:
   - `Collider2D` (for clicking)
   - `SpriteRenderer` (for visuals)
   - `Rigidbody2D` (for physics, optional)
   - `InteractableItem` script

### Step 5: Configure Items
For each item prefab:
1. Set unique `itemId` and `itemName` in InteractableItem
2. Assign the InventoryManager asset
3. Configure visual settings (sorting orders, carried offset)
4. Set game events if needed (defaults to PickedItem/DroppedItem)

## Usage Examples

### Basic Item Setup
```csharp
// On an item prefab's InteractableItem component:
// Item ID: "key_001"
// Item Name: "Rusty Key"
// Inventory Manager: [Assign your InventoryManager asset]
```

### Registering Items Programmatically
```csharp
public class ItemPlacer : MonoBehaviour 
{
    [SerializeField] private InventoryManager inventory;
    [SerializeField] private GameObject keyPrefab;
    
    void Start() 
    {
        // Register a key in the current scene
        string sceneName = SceneManager.GetActiveScene().name;
        inventory.RegisterItem("key_001", "Rusty Key", keyPrefab, 
                             new Vector2(5, 2), sceneName);
    }
}
```

### Checking Item State
```csharp
// Check if player is carrying an item
bool hasKey = inventoryManager.IsItemCarried("key_001");

// Get all items in current scene
var sceneItems = inventoryManager.GetItemsInScene(currentSceneName);

// Get all carried items
var carriedItems = inventoryManager.GetCarriedItems();
```

## Integration with Existing Systems

### Event System Integration
The system uses your existing GameEvents:
- `GameEvents.PickedItem` - When item is picked up
- `GameEvents.DroppedItem` - When item is dropped

### PlayerStatesController Integration
- Uses existing singleton pattern
- Leverages DontDestroyOnLoad functionality
- Works with scene transition system

## Debugging Tools

### InventoryManager
- **Log Inventory State**: Right-click component → "Log Inventory State"

### SceneItemSpawner
- **Refresh Scene Items**: Right-click component → "Refresh Scene Items"
- **List Items in Scene**: Right-click component → "List Items in Scene"

### PlayerInventoryController
- **List Carried Items**: Right-click component → "List Carried Items"
- **Drop All Items**: Right-click component → "Drop All Items"

### InventorySystemSetup Helper
- **Validate Setup**: Checks if all components are properly configured
- **Auto Setup**: Automatically adds missing components
- **Create Demo Items**: Creates test items for validation

## Common Issues & Solutions

### Issue: Items disappear between scenes
**Solution**: Make sure InventoryManager asset is assigned to all SceneItemSpawners

### Issue: Items duplicate when switching scenes
**Solution**: Ensure each item has a unique itemId

### Issue: Carried items don't follow player
**Solution**: Check that PlayerInventoryController is on the same GameObject as PlayerStatesController

### Issue: Items can't be clicked
**Solution**: Ensure item prefabs have Collider2D components

## Best Practices

1. **Unique IDs**: Always use unique itemId values for each item
2. **Asset References**: Use the same InventoryManager asset across all scenes
3. **Scene Organization**: Use itemParent in SceneItemSpawner for clean hierarchy
4. **Testing**: Use the InventorySystemSetup helper for validation
5. **Performance**: Don't create too many items in a single scene

## Advanced Features

### Custom Item Behaviors
Extend InteractableItem for specific item types:
```csharp
public class SpecialKey : InteractableItem 
{
    [SerializeField] private GameEvents useKeyEvent;
    
    protected override void OnMouseDown() 
    {
        if (IsCarried()) 
        {
            // Custom use logic
            Notify(useKeyEvent, transform.position);
        } 
        else 
        {
            base.OnMouseDown(); // Normal pickup
        }
    }
}
```

### Scene-Specific Logic
```csharp
public class QuestItemSpawner : MonoBehaviour 
{
    void Start() 
    {
        // Only spawn quest item if player completed previous quest
        if (PlayerPrefs.GetInt("CompletedQuest1") == 1) 
        {
            var spawner = GetComponent<SceneItemSpawner>();
            spawner.SpawnSceneItems();
        }
    }
}
```

## Migration from Existing System

If you have existing item code:
1. Replace old Item.cs with new InteractableItem.cs
2. Update item prefabs to use new component
3. Remove PlayerPrefs-based persistence code
4. Update any code that references the old system

The new system is designed to be more robust and eliminates the PlayerPrefs dependency that was in your original Item.cs implementation.