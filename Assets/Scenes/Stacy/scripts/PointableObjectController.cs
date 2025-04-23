using UnityEngine;

public class PointableObjectController : MonoBehaviour
{
    [SerializeField] private float rotationRadius = 3f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float maxRotationAngle = 45f;
    [SerializeField] private float distanceFromTarget = 2f;
    [SerializeField] GameObject target;

    private Camera mainCamera;
    private Quaternion targetRotation;


    void OnEnable()
    {
        // Get the main camera
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // Get mouse position in world space
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z; // Keep same z-position

        if (!target) return;

        // Flip the object if the target is facing left
        var isSpriteFlipX = target.GetComponent<SpriteRenderer>().flipX;
        gameObject.transform.localScale = new Vector3(isSpriteFlipX ? 1 : -1, 1, 1);

        // Calculate direction to mouse
        Vector3 direction = mousePosition - target.transform.position;

        // Adjust direction calculation based on flip state
        // if (!isSpriteFlipX)
        // {
        //     // When sprite is not flipped, invert the direction for correct rotation
        //     direction.x = -direction.x;
        // }

        // Clamp the direction to the maximum rotation radius
        if (direction.magnitude > rotationRadius)
        {
            direction = direction.normalized * rotationRadius;
        }

        // Only rotate the object, don't move it
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Limit the rotation angle
        float currentAngle = transform.eulerAngles.z;
        float clampedAngle = Mathf.Clamp(angle, -maxRotationAngle, maxRotationAngle);

        // Smoothly rotate towards the target angle
        targetRotation = Quaternion.Euler(0, 0, clampedAngle);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Ensure the object stays at its fixed position relative to the player
        transform.position = target.transform.position + new Vector3(distanceFromTarget * (isSpriteFlipX ? 1 : -1), 0, 0);
    }
}
