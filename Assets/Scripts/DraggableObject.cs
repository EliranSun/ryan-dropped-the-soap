using UnityEngine;

public class DraggableObject : ObserverSubject
{
    [SerializeField] private bool _is3D = false;

    private bool _isDragging;
    private bool _isSticky;
    private Camera _mainCamera;

    // 2D components
    private Rigidbody2D _rigidbody2D;

    // 3D components
    private Rigidbody _rigidbody3D;
    private Plane _dragPlane;
    private Vector3 _dragOffset;

    private void Start()
    {
        _mainCamera = Camera.main;

        if (_is3D)
        {
            _rigidbody3D = GetComponent<Rigidbody>();
            if (_rigidbody3D == null)
            {
                Debug.LogError("DraggableObject: 3D mode requires a Rigidbody component");
                enabled = false;
            }

            if (GetComponent<Collider>() == null)
            {
                Debug.LogError("DraggableObject: 3D mode requires a Collider component");
                enabled = false;
            }
        }
        else
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            if (_rigidbody2D == null)
            {
                Debug.LogError("DraggableObject: 2D mode requires a Rigidbody2D component");
                enabled = false;
            }

            if (GetComponent<Collider2D>() == null)
            {
                Debug.LogError("DraggableObject: 2D mode requires a Collider2D component");
                enabled = false;
            }
        }
    }

    private void Update()
    {
        if (_isSticky)
            return;

        if (_isDragging)
        {
            if (_is3D)
            {
                Handle3DDragging();
            }
            else
            {
                Handle2DDragging();
            }
        }
    }

    private void Handle2DDragging()
    {
        if (_rigidbody2D)
        {
            var mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = transform.position.z; // Maintain the original z position

            // _rigidbody2D.MovePosition(mousePosition);
            transform.position = mousePosition;
        }
    }

    private void Handle3DDragging()
    {
        if (_rigidbody3D)
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (_dragPlane.Raycast(ray, out float distance))
            {
                Vector3 targetPosition = ray.GetPoint(distance) + _dragOffset;
                // _rigidbody3D.MovePosition(targetPosition);
                transform.position = targetPosition;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_is3D && collision.relativeVelocity.magnitude > 15f)
        {
            if (_isSticky) return;
            _isDragging = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_is3D && collision.relativeVelocity.magnitude > 15f)
        {
            if (_isSticky) return;
            _isDragging = false;
        }
    }

    private void OnMouseDown()
    {
        if (_is3D)
        {
            if (_rigidbody3D)
            {
                _rigidbody3D.velocity = Vector3.zero;
                _isDragging = true;

                // Create a plane that is perpendicular to the camera view
                Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                _dragPlane = new Plane(_mainCamera.transform.forward, transform.position);

                // Calculate the offset from the hit point to the object's position
                if (_dragPlane.Raycast(ray, out float distance))
                {
                    Vector3 hitPoint = ray.GetPoint(distance);
                    _dragOffset = transform.position - hitPoint;
                }
            }
        }
        else
        {
            if (_rigidbody2D)
            {
                _rigidbody2D.velocity = Vector2.zero;
                _isDragging = true;
            }
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