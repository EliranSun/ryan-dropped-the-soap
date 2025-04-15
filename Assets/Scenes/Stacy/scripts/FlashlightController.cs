using UnityEngine;
using UnityEngine.U2D;

public class FlashlightController : MonoBehaviour
{
    [SerializeField] private Light2DBase flashlight;
    [SerializeField] private float rotationRadius = 3f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private KeyCode toggleKey = KeyCode.F;
    [SerializeField] private float maxRotationAngle = 45f;
    [SerializeField] Transform playerTransform;

    private Camera mainCamera;
    private Vector3 initialPosition;
    private bool isFlashlightOn = false;
    private Quaternion targetRotation;
    private bool _isPicked = false;

    // Start is called before the first frame update
    void Start()
    {
        // Get the main camera
        mainCamera = Camera.main;

        // Store the initial position
        initialPosition = transform.localPosition;

        // If flashlight reference is not set, try to find it on this object
        if (flashlight == null)
        {
            flashlight = GetComponent<Light2DBase>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isPicked) return;

        // Toggle flashlight on/off with F key
        if (Input.GetKeyDown(toggleKey))
        {
            isFlashlightOn = !isFlashlightOn;
            flashlight.enabled = isFlashlightOn;
        }


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
        //transform.position = playerTransform.position + initialPosition;
    }

    public void OnNotify(GameEventData eventData)
    {
        if (eventData.Name == GameEvents.FlashlightPicked)
        {
            _isPicked = true;
        }

        if (eventData.Name == GameEvents.FlashlightDropped)
        {
            _isPicked = false;
        }
    }
}
