using UnityEngine;

public class CursorManager : MonoBehaviour {
    public Texture2D CurrentTexture { get; set; }
    public bool IsActionCursor { get; set; }
    public static CursorManager Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
}