using UnityEngine;

public class CursorManager : MonoBehaviour {
    [SerializeField] private Texture2D defaultTexture;
    public Texture2D CurrentTexture { get; set; }
    public bool IsScrubbingCursor { get; set; }
    public bool IsSoapCursor { get; set; }
    public static CursorManager Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public void GrabItem(
        Texture2D texture,
        Vector2 hotSpot,
        CursorMode cursorMode,
        SpriteRenderer sprite,
        GameObject indicator
    ) {
        Cursor.SetCursor(texture, hotSpot, cursorMode);
        Instance.CurrentTexture = texture;

        print($"Textura name {texture.name}");

        // TODO: Enums
        if (texture.name == "soap-cursor")
            Instance.IsSoapCursor = true;
        if (texture.name == "sponge-cursor")
            Instance.IsScrubbingCursor = true;

        sprite.enabled = false;

        if (indicator)
            indicator.SetActive(false);
    }

    public void DropItem(SpriteRenderer sprite, GameObject indicator) {
        Cursor.SetCursor(defaultTexture, Vector2.zero, CursorMode.Auto);

        // TODO: Enums
        if (Instance.CurrentTexture.name == "soap-cursor")
            Instance.IsSoapCursor = false;
        if (Instance.CurrentTexture.name == "sponge-cursor")
            Instance.IsScrubbingCursor = false;

        Instance.CurrentTexture = defaultTexture;

        sprite.enabled = true;

        if (indicator)
            indicator.SetActive(true);
    }

    public bool CanDropItem(Texture2D texture) {
        return texture && Instance.CurrentTexture == texture;
    }
}