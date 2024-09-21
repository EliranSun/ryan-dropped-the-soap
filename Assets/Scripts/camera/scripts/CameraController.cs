using UnityEngine;

namespace camera.scripts
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private Transform targetTransform;
        [SerializeField] private float smoothTime = 0.3f;
        [SerializeField] private float followAheadDistance = 2f;
        [SerializeField] private float orthographicSizePlayer = 4f;
        [SerializeField] private float orthographicSizeElevator = 6f;
        [SerializeField] private float distanceThreshold = 7f;
        [SerializeField] private float yOffset = 1f;
        private Camera _camera;
        private float _sizeVelocity;
        private Vector3 _targetPosition;
        private Vector3 _velocity = Vector3.zero;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void FixedUpdate()
        {
            if (!targetTransform)
            {
                DefaultCameraBehaviour();
                return;
            }

            var distance = Vector2.Distance(playerTransform.position, targetTransform.position);

            if (distance >= distanceThreshold)
                FixedCameraBehaviour();
            else
                DefaultCameraBehaviour();
        }

        private void DefaultCameraBehaviour()
        {
            _targetPosition = playerTransform.position;
            _targetPosition.y += yOffset;
            _targetPosition.z = transform.position.z;

            // Smooth transition for position
            transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, smoothTime);

            // Smooth transition for orthographic size
            _camera.orthographicSize = Mathf.SmoothDamp(_camera.orthographicSize, orthographicSizePlayer,
                ref _sizeVelocity, smoothTime);
        }

        private void FixedCameraBehaviour()
        {
            // Calculate the target position ahead of the player based on the direction of movement
            var followOffset = playerTransform.right * followAheadDistance;
            _targetPosition = playerTransform.position + followOffset;
            _targetPosition.y += yOffset;
            _targetPosition.z = transform.position.z;

            // Smooth transition for position
            transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, smoothTime);

            // Smooth transition for orthographic size
            _camera.orthographicSize = Mathf.SmoothDamp(_camera.orthographicSize, orthographicSizePlayer,
                ref _sizeVelocity, smoothTime);
        }
    }
}