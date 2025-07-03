using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ParallaxLayer
{
    public Transform layerTransform;

    [Tooltip("How much this layer moves relative to mouse movement. Higher = more parallax.")]
    public float depthMultiplier = 1f;

    [HideInInspector] public Vector3 initialPosition;

    [HideInInspector] public Vector2 ParallaxOffset;
}

public class ParallaxMouseController : MonoBehaviour
{
    [Tooltip("List of layers to parallax. Assign Transforms and set depth multipliers.")]
    public List<ParallaxLayer> layers = new();

    [Tooltip("Overall sensitivity of the parallax effect.")]
    public float sensitivity = 0.1f;

    private bool initialized;

    private Vector2 screenCenter;

    private void Start()
    {
        // Store initial positions
        foreach (var layer in layers)
            if (layer.layerTransform != null)
                layer.initialPosition = layer.layerTransform.localPosition;
        screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        initialized = true;
    }

    private void Update()
    {
        if (!initialized) return;

        Vector2 mousePos = Input.mousePosition;
        var offsetFromCenter = (mousePos - screenCenter) / screenCenter; // -1 to 1 range

        foreach (var layer in layers)
        {
            if (layer.layerTransform == null) continue;
            layer.ParallaxOffset = new Vector2(
                offsetFromCenter.x * layer.depthMultiplier * sensitivity,
                offsetFromCenter.y * layer.depthMultiplier * sensitivity
            );
        }
    }
}