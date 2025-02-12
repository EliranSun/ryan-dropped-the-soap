using UnityEngine;

namespace common.scripts
{
    public class ParallaxEffect : MonoBehaviour
    {
        [Header("References")] public Transform cameraTransform;

        [Header("Parallax Settings")]
        [Tooltip(
            "Set this between 0 and 1 for background layers (e.g., 0.5). Use values greater than 1 for foreground layers.")]
        public float parallaxMultiplier = 0.5f;

        public float yOffset;
        public bool disableYAxis;
        private Vector3 _lastCameraPosition;
        private float _originalY;


        private void Start()
        {
            // Initialize the last camera position
            if (cameraTransform == null) cameraTransform = Camera.main.transform;
            _lastCameraPosition = cameraTransform.position;
            _originalY = transform.position.y;
        }

        private void LateUpdate()
        {
            // Calculate the camera's movement since the last frame
            var deltaMovement = cameraTransform.position - _lastCameraPosition;

            // Move the background or foreground object
            transform.position += deltaMovement * parallaxMultiplier;
            if (disableYAxis) transform.position = new Vector3(transform.position.x, _originalY, transform.position.z);
            if (yOffset != 0) transform.position = new Vector3(transform.position.x, _originalY + yOffset, transform.position.z);

            // Update the last camera position
            _lastCameraPosition = cameraTransform.position;
        }
    }
}