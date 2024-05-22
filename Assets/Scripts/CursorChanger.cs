using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CursorChanger : MonoBehaviour {
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Texture2D defaultTexture;
    [SerializeField] private Vector2 hotSpot = Vector2.zero;
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;
    private SpriteRenderer _spriteRenderer;

    private void Start() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        // TODO: Move this to CursorManager, once
        Cursor.SetCursor(defaultTexture, hotSpot, cursorMode);
        CursorManager.Instance.CurrentTexture = defaultTexture;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(1))
            if (CanDropCursorItem()) {
                Cursor.SetCursor(defaultTexture, hotSpot, cursorMode);
                CursorManager.Instance.CurrentTexture = defaultTexture;
                CursorManager.Instance.IsActionCursor = false;
                _spriteRenderer.enabled = true;
            }
    }

    private void OnMouseDown() {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        CursorManager.Instance.CurrentTexture = cursorTexture;
        CursorManager.Instance.IsActionCursor = true;
        _spriteRenderer.enabled = false;
    }

    private bool CanDropCursorItem() {
        return cursorTexture && CursorManager.Instance.CurrentTexture == cursorTexture;
    }
}