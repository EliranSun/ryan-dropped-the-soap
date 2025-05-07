using UnityEngine;

namespace Scenes.Stacy.scripts
{
    public class CameraSectionMovement : MonoBehaviour
    {
        [SerializeField] private Transform target;

        private Camera _camera;
        private float _cameraHeight;
        private float _cameraWidth;
        private Vector3 _currentSection;
        private Vector3 _lastPosition;

        private void Start()
        {
            _camera = GetComponent<Camera>();
            UpdateCameraData();
        }

        private void UpdateCameraData()
        {
            _cameraWidth = _camera.orthographicSize * _camera.aspect * 2;
            _cameraHeight = _camera.orthographicSize * 2;

            // Initialize camera position based on target
            // AlignCameraWithTarget();

            // Store the camera's section position
            _currentSection = transform.position;

            // Remember the target's position
            _lastPosition = target.position;
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.ZoomChangeEnd) {
                UpdateCameraData();
            }
        }

        private void LateUpdate()
        {
            // Check if the target has moved outside the camera boundaries
            CheckBoundaries();

            // Update the last known position of the target
            _lastPosition = target.position;
        }

        private void AlignCameraWithTarget()
        {
            // Calculate the section based on the target's position
            var sectionX = Mathf.Floor(target.position.x / _cameraWidth) * _cameraWidth + _cameraWidth / 2;
            var sectionY = Mathf.Floor(target.position.y / _cameraHeight) * _cameraHeight + _cameraHeight / 2;

            // Set the camera position to the center of the section
            // Maintain the camera's z position for proper rendering
            transform.position = new Vector3(sectionX, sectionY, transform.position.z);
        }

        private void CheckBoundaries()
        {
            // Calculate the half sizes for easier boundary checks
            var halfWidth = _cameraWidth / 2;
            var halfHeight = _cameraHeight / 2;

            // Get the current camera position (center of section)
            var cameraPosition = _currentSection;
            var needsUpdate = false;

            // Check if the target has moved to the right or left section
            if (target.position.x > cameraPosition.x + halfWidth)
            {
                // Move camera one section to the right
                cameraPosition.x += _cameraWidth;
                needsUpdate = true;
            }
            else if (target.position.x < cameraPosition.x - halfWidth)
            {
                // Move camera one section to the left
                cameraPosition.x -= _cameraWidth;
                needsUpdate = true;
            }

            // Check if the target has moved to the upper or lower section
            if (target.position.y > cameraPosition.y + halfHeight)
            {
                // Move camera one section up
                cameraPosition.y += _cameraHeight;
                needsUpdate = true;
            }
            else if (target.position.y < cameraPosition.y - halfHeight)
            {
                // Move camera one section down
                cameraPosition.y -= _cameraHeight;
                needsUpdate = true;
            }

            // Update camera position if needed
            if (needsUpdate)
            {
                _currentSection = cameraPosition;
                transform.position = new Vector3(cameraPosition.x, cameraPosition.y, transform.position.z);
            }
        }
    }
}