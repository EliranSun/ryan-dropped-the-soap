using UnityEngine;

public class PointableObjectController : MonoBehaviour
{
    [SerializeField] private float rotationRadius = 3f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float maxRotationAngle = 45f;
    [SerializeField] Transform playerTransform;

    private Camera mainCamera;
    private Vector3 initialPosition;
    private bool isFlashlightOn = false;
    private Quaternion targetRotation;

    // Start is called before the first frame update
    void Start()
    {
        // Get the main camera
        mainCamera = Camera.main;

        // Store the initial position
        initialPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        // Get mouse position in world space
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z; // Keep same z-position

        if (!playerTransform) return;

        // Calculate direction to mouse
        Vector3 direction = mousePosition - playerTransform.position;

        // Clamp the direction to the maximum rotation radius
        if (direction.magnitude > rotationRadius)
        {
            direction = direction.normalized * rotationRadius;
        }

        // Only rotate the flashlight, don't move it
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Optional: Limit the rotation angle if desired
        float currentAngle = transform.eulerAngles.z;
        float clampedAngle = Mathf.Clamp(angle, -maxRotationAngle, maxRotationAngle);

        // Smoothly rotate towards the target angle
        targetRotation = Quaternion.Euler(0, 0, clampedAngle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Ensure the flashlight stays at its fixed position relative to the player
        transform.position = playerTransform.position + initialPosition;
    }
}
