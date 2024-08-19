using UnityEngine;

namespace camera.scripts
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private float smoothTime = 0.3f;
        [SerializeField] private float followAheadDistance = 2f;
        [SerializeField] private float orthographicSizePlayer = 4f;
        [SerializeField] private float orthographicSizeElevator = 6f;
        private float sizeVelocity;
        private Vector3 targetPosition;

        private Vector3 velocity = Vector3.zero;

        private void FixedUpdate()
        {
            var distance = Vector2.Distance(_playerTransform.position, _targetTransform.position);
            if (distance >= 7f)
            {
                // Calculate the target position ahead of the player based on the direction of movement
                var followOffset = _playerTransform.right * followAheadDistance;
                targetPosition = _playerTransform.position + followOffset;
                targetPosition.z = transform.position.z;

                // Smooth transition for position
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

                // Smooth transition for orthographic size
                Camera.main.orthographicSize = Mathf.SmoothDamp(Camera.main.orthographicSize, orthographicSizePlayer,
                    ref sizeVelocity, smoothTime);
            }
            else
            {
                targetPosition = _targetTransform.position;
                targetPosition.z = transform.position.z;

                // Smooth transition for position
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

                // Smooth transition for orthographic size
                Camera.main.orthographicSize = Mathf.SmoothDamp(Camera.main.orthographicSize, orthographicSizeElevator,
                    ref sizeVelocity, smoothTime);
            }
        }
    }
}