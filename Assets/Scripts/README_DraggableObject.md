# DraggableObject Component

This component allows for mouse-based dragging of both 2D and 3D objects in Unity.

## Features

- Supports both 2D and 3D objects
- Handles physics interactions
- Supports "sticky" mode through the observer pattern
- Automatically validates required components

## Setup for 2D Objects

1. Attach a `Collider2D` component to your GameObject
2. Attach a `Rigidbody2D` component to your GameObject
3. Add the `DraggableObject` component
4. Ensure the `_is3D` checkbox is **unchecked**

## Setup for 3D Objects

1. Attach a `Collider` component to your GameObject
2. Attach a `Rigidbody` component to your GameObject
3. Add the `DraggableObject` component
4. Check the `_is3D` checkbox in the Inspector

## Observer Pattern Integration

The `DraggableObject` script inherits from `ObserverSubject` and responds to the following game events:

- `GameEvents.TriggerStick`: Makes the object "sticky" (prevents dragging)
- `GameEvents.TriggerNonStick`: Makes the object draggable again

## Notes

- For 3D objects, dragging occurs on a plane perpendicular to the camera view
- Objects will stop being dragged if they collide with something at a high velocity (> 15 units/second)
- The script will log errors if the required components are missing 