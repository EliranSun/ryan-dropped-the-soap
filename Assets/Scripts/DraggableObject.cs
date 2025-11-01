using UnityEngine;

public class DraggableObject : ObserverSubject
{
    [SerializeField] private GameEvents onDragEvent;
    [SerializeField] private GameEvents onDropEvent;
    [SerializeField] private bool _is3D;
    [SerializeField] private LayerMask draggableLayerMask = -1; // Which layers can be dragged
    public bool destroyOnDrop;
    private Collider2D _collider2D;

    // Manual input handling
    private Collider _collider3D;
    private Vector3 _dragOffset;
    private Plane _dragPlane;

    private bool _isDragging;
    private bool _isSticky;
    private Camera _mainCamera;

    // 2D components
    private Rigidbody2D _rigidbody2D;

    // 3D components
    private Rigidbody _rigidbody3D;

    private void Start()
    {
        _mainCamera = Camera.main;

        if (_is3D)
        {
            _rigidbody3D = GetComponent<Rigidbody>();
            _collider3D = GetComponent<Collider>();

            if (_rigidbody3D == null)
            {
                Debug.LogError("DraggableObject: 3D mode requires a Rigidbody component");
                enabled = false;
            }

            if (_collider3D == null)
            {
                Debug.LogError("DraggableObject: 3D mode requires a Collider component");
                enabled = false;
            }
        }
        else
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _collider2D = GetComponent<Collider2D>();

            if (_rigidbody2D == null)
            {
                Debug.LogError("DraggableObject: 2D mode requires a Rigidbody2D component");
                enabled = false;
            }

            if (_collider2D == null)
            {
                Debug.LogError("DraggableObject: 2D mode requires a Collider2D component");
                enabled = false;
            }
        }
    }

    private void Update()
    {
        HandleMouseInput();

        if (_isSticky)
            return;

        if (_isDragging)
        {
            if (_is3D)
                Handle3DDragging();
            else
                Handle2DDragging();
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_is3D && collision.relativeVelocity.magnitude > 15f)
        {
            if (_isSticky) return;
            _isDragging = false;
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsMouseOverThisObject()) StartDragging();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (_isDragging) StopDragging();
        }
    }

    private bool IsMouseOverThisObject()
    {
        if (_is3D)
        {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            var hits = Physics.RaycastAll(ray, Mathf.Infinity, draggableLayerMask);

            // Check if this object is the closest draggable object
            var closestDistance = Mathf.Infinity;
            GameObject closestDraggable = null;

            foreach (var hit in hits)
                if (hit.collider.GetComponent<DraggableObject>() != null && hit.distance < closestDistance)
                {
                    closestDistance = hit.distance;
                    closestDraggable = hit.collider.gameObject;
                }

            return closestDraggable == gameObject;
        }
        else
        {
            Vector2 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var hits = Physics2D.RaycastAll(mouseWorldPos, Vector2.zero, 0f, draggableLayerMask);

            // Check if this object is the topmost draggable object (highest sorting order or closest to camera)
            var closestZ = Mathf.Infinity;
            GameObject closestDraggable = null;

            foreach (var hit in hits)
                if (hit.collider.GetComponent<DraggableObject>() != null)
                {
                    var zPos = hit.collider.transform.position.z;
                    if (zPos < closestZ) // In 2D, smaller Z values are closer to camera
                    {
                        closestZ = zPos;
                        closestDraggable = hit.collider.gameObject;
                    }
                }

            return closestDraggable == gameObject;
        }
    }

    private void StartDragging()
    {
        if (_is3D)
        {
            if (_rigidbody3D)
            {
                _rigidbody3D.linearVelocity = Vector3.zero;
                _isDragging = true;
                Notify(onDragEvent);

                // Create a plane that is perpendicular to the camera view
                var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                _dragPlane = new Plane(_mainCamera.transform.forward, transform.position);

                // Calculate the offset from the hit point to the object's position
                if (_dragPlane.Raycast(ray, out var distance))
                {
                    var hitPoint = ray.GetPoint(distance);
                    _dragOffset = transform.position - hitPoint;
                }
            }
        }
        else
        {
            if (_rigidbody2D)
            {
                _rigidbody2D.linearVelocity = Vector2.zero;
                _isDragging = true;
                Notify(onDragEvent);
            }
        }
    }

    private void StopDragging()
    {
        _isDragging = false;
        Notify(onDropEvent);

        if (destroyOnDrop) Destroy(gameObject);
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
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (_dragPlane.Raycast(ray, out var distance))
            {
                var targetPosition = ray.GetPoint(distance) + _dragOffset;
                // _rigidbody3D.MovePosition(targetPosition);
                transform.position = targetPosition;
            }
        }
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