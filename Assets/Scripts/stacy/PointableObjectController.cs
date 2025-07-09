using UnityEngine;

namespace stacy
{
    public class PointableObjectController : MonoBehaviour
    {
        [SerializeField] private float rotationRadius = 3f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float maxRotationAngle = 45f;
        [SerializeField] private float distanceFromTarget = 2f;
        [SerializeField] private GameObject target;

        private Camera mainCamera;
        private Quaternion targetRotation;

        private void Update()
        {
            var mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = transform.position.z; // Keep same z-position

            if (!target) return;

            var isSpriteFlipX = target.GetComponent<SpriteRenderer>().flipX;
            gameObject.transform.localScale = new Vector3(isSpriteFlipX ? 1 : -1, 1, 1);

            var direction = mousePosition - target.transform.position;

            if (direction.magnitude > rotationRadius) direction = direction.normalized * rotationRadius;

            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            var currentAngle = transform.eulerAngles.z;
            var clampedAngle = Mathf.Clamp(angle, -maxRotationAngle, maxRotationAngle);

            targetRotation = Quaternion.Euler(0, 0, clampedAngle);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            transform.position = target.transform.position +
                                 new Vector3(distanceFromTarget * (isSpriteFlipX ? 1 : -1), 0, 0);
        }


        private void OnEnable()
        {
            // Get the main camera
            mainCamera = Camera.main;
        }
    }
}