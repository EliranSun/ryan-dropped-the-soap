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
        private float _sizeVelocity;
        private Vector3 _targetPosition;
        private Vector3 _velocity = Vector3.zero;

        private void FixedUpdate()
        {
            var distance = Vector2.Distance(playerTransform.position, targetTransform.position);
            if (distance >= 7f)
            {
                // Calculate the target position ahead of the player based on the direction of movement
                var followOffset = playerTransform.right * followAheadDistance;
                _targetPosition = playerTransform.position + followOffset;
                _targetPosition.z = transform.position.z;

                // Smooth transition for position
                transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, smoothTime);

                // Smooth transition for orthographic size
                Camera.main.orthographicSize = Mathf.SmoothDamp(Camera.main.orthographicSize, orthographicSizePlayer,
                    ref _sizeVelocity, smoothTime);
            }
            else
            {
                _targetPosition = targetTransform.position;
                _targetPosition.z = transform.position.z;

                // Smooth transition for position
                transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, smoothTime);

                // Smooth transition for orthographic size
                Camera.main.orthographicSize = Mathf.SmoothDamp(Camera.main.orthographicSize, orthographicSizeElevator,
                    ref _sizeVelocity, smoothTime);
            }
        }
    }
}