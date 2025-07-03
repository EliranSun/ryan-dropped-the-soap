using UnityEngine;

public class UIEdgePan : MonoBehaviour
{
    [Tooltip("Width of the screen edge (in pixels) that triggers panning.")]
    public float edgeWidth = 50f;

    [Tooltip("Speed at which the camera object rotates")]
    public float rotationSpeed = 1f;

    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        var move = 0f;

        if (Input.GetKey(KeyCode.A))
            move = -1f;
        else if (Input.GetKey(KeyCode.D))
            move = 1f;

        print($"Move {move}");

        if (move != 0f)
            // rotate the Y axis of the camera
            _mainCamera.transform.Rotate(new Vector3(0, rotationSpeed * move, 0));
    }
}