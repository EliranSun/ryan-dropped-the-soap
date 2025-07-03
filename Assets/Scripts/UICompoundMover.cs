using UnityEngine;

public class UICompoundMover : MonoBehaviour
{
    [Tooltip("Reference to the ParallaxMouseController script.")]
    public ParallaxMouseController parallaxScript;

    [Tooltip("Index of the parallax layer to use from ParallaxMouseController.")]
    public int parallaxLayerIndex;

    private Vector2 initialPosition;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        initialPosition = rectTransform.anchoredPosition;
    }

    private void Update()
    {
        var totalOffset = Vector2.zero;

        if (parallaxScript != null &&
            parallaxScript.layers != null &&
            parallaxLayerIndex >= 0 &&
            parallaxLayerIndex < parallaxScript.layers.Count)
            totalOffset += parallaxScript.layers[parallaxLayerIndex].ParallaxOffset;
        rectTransform.anchoredPosition = initialPosition + totalOffset;
    }
}