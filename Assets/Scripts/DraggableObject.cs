using UnityEngine;

public class DraggableObject : ObserverSubject {
    private bool _isDragging;
    private bool _isSticky;
    private Camera _mainCamera;
    private Rigidbody2D _rigidbody2D;

    private void Start() {
        _mainCamera = Camera.main;
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        if (_isSticky)
            return;

        if (_isDragging) {
            var mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f; // Ensure we only work with 2D coordinates

            // Move the object by setting the Rigidbody2D's position
            _rigidbody2D.MovePosition(mousePosition);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.relativeVelocity.magnitude > 15f) {
            if (_isSticky) return;
            _isDragging = false;
        }
    }

    private void OnMouseDown() {
        _isDragging = true;
    }

    private void OnMouseUp() {
        _isDragging = false;
    }

    public void OnNotify(GameEventData gameEventData) {
        _isSticky = gameEventData.name switch {
            GameEvents.TriggerStick => true,
            GameEvents.TriggerNonStick => false,
            _ => _isSticky
        };
    }
}