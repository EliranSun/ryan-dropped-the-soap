using UnityEngine;

public class FaucetChange : MonoBehaviour {
    [SerializeField] private float rotateNotifyDebounce = 0.5f;
    [SerializeField] private float sensitivity = 1;
    private float _currentAngle;
    private int _faucetOpenLevel;
    private Camera _mainCamera;
    private Vector3 _objectCenter;
    private Debouncer notifyFaucetClose;
    private Debouncer notifyFaucetOpen;

    private void Start() {
        _mainCamera = Camera.main;

        notifyFaucetOpen = gameObject.AddComponent<Debouncer>();
        notifyFaucetClose = gameObject.AddComponent<Debouncer>();

        notifyFaucetOpen.Setup(rotateNotifyDebounce, () => {
            if (_faucetOpenLevel > 5)
                return;

            EventManager.Instance.Publish(GameEvents.FaucetOpening);
            _faucetOpenLevel++;
        });

        notifyFaucetClose.Setup(rotateNotifyDebounce, () => {
            if (_faucetOpenLevel == 0)
                return;

            EventManager.Instance.Publish(GameEvents.FaucetClosing);
            _faucetOpenLevel--;

            if (_faucetOpenLevel == 0)
                EventManager.Instance.Publish(GameEvents.FaucetClosed);
        });
    }

    private void OnMouseDown() {
        _objectCenter = transform.position;
        _currentAngle = AngleBetweenPoints(_mainCamera.ScreenToWorldPoint(Input.mousePosition), _objectCenter);
    }

    private void OnMouseDrag() {
        if (CursorManager.Instance.IsActionCursor)
            return;

        var newMousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var newAngle = AngleBetweenPoints(newMousePosition, _objectCenter);
        var angleDifference = (newAngle - _currentAngle) * sensitivity;

        switch (_faucetOpenLevel) {
            // trying to close but already closed
            case <= 0 when newAngle > _currentAngle:
            // trying to open but already maxed
            case > 5 when newAngle < _currentAngle:
                return;
        }

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