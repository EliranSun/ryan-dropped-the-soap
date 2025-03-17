using System.Collections;
using UnityEngine;

namespace Object.Scripts
{
    [RequireComponent(typeof(Camera))]
    public class ObjectFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float xOffset;
        [SerializeField] private float yOffset;
        [SerializeField] private bool lockZ;
        [SerializeField] private bool lockY;
        [SerializeField] private int centerOnObjectDelay;
        [SerializeField] private int transitionDuration = 5;
        private float _initialZ;
        private Camera _mainCamera;
        private bool _centerOnObject = false;

        private void Start()
        {
            _mainCamera = GetComponent<Camera>();
            _initialZ = transform.position.z;

            if (centerOnObjectDelay > 0)
            {
                Invoke(nameof(CenterOnObject), centerOnObjectDelay);
            }
        }

        private void CenterOnObject()
        {
            var newPosition = target.position;
            newPosition.y += yOffset;
            if (lockZ) newPosition.z = _initialZ;
            if (lockY) newPosition.y = yOffset;
            StartCoroutine(SmoothCameraTransition(newPosition));
        }

        private void Update()
        {
            var newPosition = target.position;
            newPosition.y += yOffset;

            if (!_centerOnObject)
                newPosition.x += xOffset;

            if (lockZ) newPosition.z = _initialZ;
            if (lockY) newPosition.y = yOffset;

            transform.position = newPosition;
        }

        private IEnumerator SmoothCameraTransition(Vector3 targetPosition)
        {
            var elapsedTime = 0f;
            var startPosition = _mainCamera.gameObject.transform.position;

            while (elapsedTime < transitionDuration)
            {
                // Update target position to account for object movement
                var currentTargetPosition = target.position;
                currentTargetPosition.y += yOffset;
                if (lockZ) currentTargetPosition.z = _initialZ;
                if (lockY) currentTargetPosition.y = yOffset;

                elapsedTime += Time.deltaTime;
                var t = elapsedTime / transitionDuration;
                _mainCamera.gameObject.transform.position = Vector3.Lerp(startPosition, currentTargetPosition, t);
                yield return null;
            }

            // Final position should be the current target position
            var finalPosition = target.position;
            finalPosition.y += yOffset;
            if (lockZ) finalPosition.z = _initialZ;
            if (lockY) finalPosition.y = yOffset;

            _mainCamera.gameObject.transform.position = finalPosition;
            _centerOnObject = true;
        }
    }
}