# Item Duplication Fix Guide

## What Was Fixed

The duplication issue you experienced was caused by:
1. **DontDestroyOnLoad items persisting** across scene reloads
2. **Multiple spawning systems** trying to create the same items
3. **Lack of centralized tracking** of spawned item instances

## Key Changes Made

### 1. **Removed DontDestroyOnLoad from Individual Items**
- Items no longer persist individually across scenes
- Only the `InventoryManager` (ScriptableObject) and `PlayerInventoryController` persist
- This eliminates the main source of duplication

### 2. **Centralized Instance Tracking**
```csharp
// InventoryManager now tracks all spawned instances
private static Dictionary<string, GameObject> spawnedItemInstances = new();

public void RegisterSpawnedItem(string itemId, GameObject spawnedObject)
public bool IsItemSpawned(string itemId)
```

### 3. **Enhanced Duplicate Prevention**
- **Multiple layers of checking** before spawning items
- **Cleanup systems** that remove null references
- **Scene-aware spawning** that respects current item states

### 4. **Better Scene Transition Handling**
- **PlayerInventoryController** recreates carried items after scene load
- **SceneItemSpawner** has comprehensive duplicate checking
- **Timing coordination** with delays to ensure proper initialization order

## How to Use the Fixed System

### 1. Setup (Same as Before)
1. Create an `InventoryManager` asset
2. Add `PlayerInventoryController` to your player
3. Add `SceneItemSpawner` to each scene
4. Create item prefabs with `InteractableItem` component

### 2. Key Differences
- **No more DontDestroyOnLoad** on item prefabs
- **Items are recreated** on scene transitions, not persisted
- **State is managed centrally** in the ScriptableObject

## Debugging Tools

### If You Still See Duplicates:

#### 1. Check InventoryManager State
```csharp
// Right-click InventoryManager component → "Log Inventory State"
// This shows what the system thinks exists
```

#### 2. Check Scene Item Spawner
```csharp
// Right-click SceneItemSpawner → "List Items in Scene"
// This compares what should exist vs what actually exists
```

#### 3. Force Cleanup
```csharp
// Right-click InventoryManager → "Force Cleanup"
// Right-click SceneItemSpawner → "Force Cleanup Scene"
```

#### 4. Check Player Inventory
```csharp
// Right-click PlayerInventoryController → "List Carried Items"
// This shows what the player should be carrying vs what actually exists
```

## Common Issues & Solutions

### Issue: Items still duplicate on scene restart
**Solution:** 
- Use "Force Cleanup" context menus
- Make sure you're using the new scripts, not old ones
- Check that item prefabs don't have DontDestroyOnLoad

### Issue: Carried items disappear between scenes
**Solution:**
- Ensure PlayerInventoryController has the InventoryManager assigned
- Check the Console for "Restored carried item" messages
- Use "Force Restore Carried Items" context menu

### Issue: Items spawn in wrong locations
**Solution:**
- Check the InventoryManager asset to see stored positions
- Items dropped in Scene A should appear in Scene A when you return

### Issue: Items can't be picked up
**Solution:**
- Ensure item prefabs have Collider2D components
- Check that InteractableItem has the InventoryManager assigned

## System Flow

1. **Scene Loads** → SceneItemSpawner awakens → Cleans up old references
2. **SceneItemSpawner.Start()** → Spawns items that should be in this scene
3. **PlayerInventoryController.Start()** → Restores carried items from InventoryManager
4. **Player clicks item** → Item updates InventoryManager → Visual state changes
5. **Scene Changes** → Carried items destroyed → InventoryManager retains state
6. **New Scene Loads** → Process repeats

## Performance Notes

- **No more PlayerPrefs** for item storage (faster)
- **Centralized state** reduces duplicate checking
- **Smart spawning** only creates items when needed
- **Automatic cleanup** prevents memory leaks

## Migration from Old System

If you have existing saves/items:
1. **Reset inventory** using InventoryManager → "Reset Inventory"
2. **Clear scene items** using SceneItemSpawner → "Force Cleanup Scene"
3. **Start fresh** - the new system is incompatible with old PlayerPrefs data

## Debug Console Messages

When working properly, you should see:
```
SceneItemSpawner: Spawned 3/3 items in scene: hallway scene
PlayerInventoryController: Attempting to restore 1 carried items
Restored carried item: Magic Key
```

If you see duplication warnings:
```
Item Magic Key already exists in scene, registering existing instance
Item Magic Key already tracked as spawned, skipping
```

This is **normal** and means the system is working correctly to prevent duplicates!

## Need More Help?

Use the context menu debug tools - they provide detailed information about what the system thinks should exist vs. what actually exists. The console output will help you identify any remaining issues.