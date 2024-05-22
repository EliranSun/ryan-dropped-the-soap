using UnityEngine;

public class Zoom : MonoBehaviour {
    [SerializeField] private float startSize = 1;
    [SerializeField] private float endSize = 6;
    private Camera _mainCamera;

    public static Zoom Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start() {
        _mainCamera = Camera.main;
        _mainCamera.orthographicSize = startSize;
    }

    private void Update() {
        if (_mainCamera.orthographicSize >= endSize)
            return;

        _mainCamera.orthographicSize += Time.deltaTime;
    }
}