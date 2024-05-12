using UnityEngine;

public class HandleRotation : ObserverSubject {
    [SerializeField] private float sensitivity = 1;
    private float _currentAngle;
    private Camera _mainCamera;
    private Vector3 _objectCenter;
    private Debouncer notifyFaucetClose;
    private Debouncer notifyFaucetOpen;

    private void Start() {
        _mainCamera = Camera.main;
        notifyFaucetOpen = gameObject.AddComponent<Debouncer>();
        notifyFaucetClose = gameObject.AddComponent<Debouncer>();
        notifyFaucetOpen.Setup(1, () => Notify(GameEvents.FaucetOpening));
        notifyFaucetClose.Setup(1, () => Notify(GameEvents.FaucetClosing));
    }


    private void OnMouseDown() {
        _objectCenter = transform.position;
        _currentAngle = AngleBetweenPoints(_mainCamera.ScreenToWorldPoint(Input.mousePosition), _objectCenter);
    }

    private void OnMouseDrag() {
        var newMousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var newAngle = AngleBetweenPoints(newMousePosition, _objectCenter);
        var angleDifference = (newAngle - _currentAngle) * sensitivity;

        transform.RotateAround(_objectCenter, Vector3.forward, angleDifference);

        if (newAngle != _currentAngle)
            if (newAngle < _currentAngle)
                notifyFaucetOpen.Debounce();
            else
                notifyFaucetClose.Debounce();

        _currentAngle = newAngle;
    }

    private float AngleBetweenPoints(Vector3 a, Vector3 b) {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }
}