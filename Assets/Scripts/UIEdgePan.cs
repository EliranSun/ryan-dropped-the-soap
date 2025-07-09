using UnityEngine;
using UnityEngine.InputSystem;

public class UIEdgePan : MonoBehaviour
{
    [Tooltip("Speed at which the camera object rotates")]
    public float rotationSpeed = 1f;

    private Camera _mainCamera;
    private InputAction _panAction;

    private void Start()
    {
        _mainCamera = Camera.main;
        _panAction = InputSystem.actions.FindAction("Move");
    }

    private void Update()
    {
        var moveValue = _panAction.ReadValue<Vector2>();
        if (moveValue.x != 0f)
            // rotate the Y axis of the camera
            _mainCamera.transform.Rotate(new Vector3(0, rotationSpeed * moveValue.x, 0));
    }
}