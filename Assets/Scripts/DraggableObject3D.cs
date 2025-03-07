using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class DraggableObject3D : ObserverSubject
{
    private float _distanceToCamera;
    private Vector3 _dragOffset;
    private Plane _dragPlane;
    private bool _isDragging;
    private bool _isSticky;
    private Camera _mainCamera;
    private Rigidbody _rigidbody;

    private void Start()
    {
        _mainCamera = Camera.main;
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_isSticky)
            return;

        if (_isDragging && _rigidbody)
        {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (_dragPlane.Raycast(ray, out var distance))
            {
                var targetPosition = ray.GetPoint(distance) + _dragOffset;
                _rigidbody.MovePosition(targetPosition);
                // transform.position = targetPosition;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 15f)
        {
            if (_isSticky) return;
            _isDragging = false;
        }
    }

    private void OnMouseDown()
    {
        _rigidbody.velocity = Vector3.zero;
        _isDragging = true;

        // Create a plane that is perpendicular to the camera view
        var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        _dragPlane = new Plane(_mainCamera.transform.forward, transform.position);

        // Calculate the distance from the camera to the object
        if (_dragPlane.Raycast(ray, out var distance))
        {
            var hitPoint = ray.GetPoint(distance);
            _dragOffset = transform.position - hitPoint;
        }
    }

    private void OnMouseUp()
    {
        _isDragging = false;
    }

    public void OnNotify(GameEventData gameEventData)
    {
        _isSticky = gameEventData.Name switch
        {
            GameEvents.TriggerStick => true,
            GameEvents.TriggerNonStick => false,
            _ => _isSticky
        };
    }
}