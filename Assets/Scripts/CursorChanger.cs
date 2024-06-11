using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CursorChanger : MonoBehaviour {
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Texture2D defaultTexture;
    [SerializeField] private Vector2 hotSpot = Vector2.zero;
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;
    [SerializeField] private GameObject indicatorGameObject;
    [SerializeField] private bool triggeredDirectly = true;
    [SerializeField] private bool triggerOnMouseEnter;
    private bool _isHolding;
    private SpriteRenderer _spriteRenderer;

    private void Start() {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor) {
            // Scaling for mac, DPI issues i guess
            var resizedTexture = ResizeTexture(
                defaultTexture,
                defaultTexture.width / 10,
                defaultTexture.height / 10);

            defaultTexture = resizedTexture;
        }

        // TODO: Move this to CursorManager, once
        Cursor.SetCursor(defaultTexture, hotSpot, cursorMode);
        CursorManager.Instance.CurrentTexture = defaultTexture;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(1))
            if (CursorManager.Instance.CanDropItem(cursorTexture))
                CursorManager.Instance.DropItem(_spriteRenderer, indicatorGameObject);
    }

    private void OnMouseDown() {
        _isHolding = true;
        if (!triggeredDirectly || triggerOnMouseEnter)
            return;

        TriggerGrab();
    }

    private void OnMouseEnter() {
        if (!triggerOnMouseEnter)
            return;


        TriggerGrab();
    }

    private void OnMouseExit() {
        if (!triggerOnMouseEnter)
            return;

        if (_isHolding)
            return;

        if (CursorManager.Instance.CanDropItem(cursorTexture))
            CursorManager.Instance.DropItem(_spriteRenderer, indicatorGameObject);
    }

    private void OnMouseUp() {
        _isHolding = false;

        if (triggerOnMouseEnter && CursorManager.Instance.CanDropItem(cursorTexture))
            CursorManager.Instance.DropItem(_spriteRenderer, indicatorGameObject);
    }

    public void TriggerGrab() {
        CursorManager.Instance.GrabItem(
            cursorTexture,
            hotSpot,
            cursorMode,
            _spriteRenderer,
            indicatorGameObject,
            !triggerOnMouseEnter
        );
    }

    private Texture2D ResizeTexture(Texture2D original, int newWidth, int newHeight) {
        var newTexture = new Texture2D(newWidth, newHeight);
        var pixels = original.GetPixels(0, 0, original.width, original.height);
        var newPixels = new Color[newWidth * newHeight];

        var ratioX = (float)original.width / newWidth;
        var ratioY = (float)original.height / newHeight;

        for (var y = 0; y < newHeight; y++)
        for (var x = 0; x < newWidth; x++)
            newPixels[y * newWidth + x] = pixels[(int)(y * ratioY) * original.width + (int)(x * ratioX)];

        newTexture.SetPixels(newPixels);
        newTexture.Apply();
        return newTexture;
    }
}