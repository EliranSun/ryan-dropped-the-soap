using System.Collections;
using UnityEngine;

namespace Object.Scripts
{
    [RequireComponent(typeof(Camera))]
    public class CameraObjectFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float xOffset;
        [SerializeField] private float yOffset;
        [SerializeField] private bool lockZ;
        [SerializeField] private bool lockY;
        [SerializeField] private int centerOnObjectDelay;
        [SerializeField] private int transitionDuration = 5;
        [SerializeField] private bool catchUpMode = false;
        [SerializeField] private float catchUpSpeed = 5f;
        private bool _centerOnObject;
        private float _initialZ;
        private bool _isActive;
        private Camera _mainCamera;
        // For SmoothDamp
        private Vector3 _velocity = Vector3.zero;
        private Vector3 _lastTargetPosition;
        private bool _isCatchingUp = false;

        private void Start()
        {
            _mainCamera = GetComponent<Camera>();
            _initialZ = transform.position.z;
            _lastTargetPosition = target ? target.position : Vector3.zero;

            if (centerOnObjectDelay > 0) Invoke(nameof(CenterOnObject), centerOnObjectDelay);
        }

        private void Update()
        {
            if (!_isActive || !target) return;

            if (catchUpMode)
            {
                Vector3 targetPos = target.position;
                targetPos.y += yOffset;
                if (lockZ) targetPos.z = _initialZ;
                if (lockY) targetPos.y = yOffset;

                // Check if target is moving
                bool isMoving = (target.position - _lastTargetPosition).sqrMagnitude > 0.001f;

                if (isMoving)
                {
                    // Camera follows with lag
                    transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _velocity, 1f / catchUpSpeed);
                    _isCatchingUp = true;
                }
                else if (_isCatchingUp)
                {
                    // When target stops, recenter smoothly
                    _isCatchingUp = false;
                    StopAllCoroutines();
                    StartCoroutine(SmoothCameraTransition(targetPos));
                }
                _lastTargetPosition = target.position;
                return;
            }

            var newPosition = target.position;
            newPosition.y += yOffset;

            if (!_centerOnObject)
                newPosition.x += xOffset;

            if (lockZ) newPosition.z = _initialZ;
            if (lockY) newPosition.y = yOffset;

            transform.position = newPosition;
        }

        private void OnEnable()
        {
            _isActive = true;
            _lastTargetPosition = target ? target.position : Vector3.zero;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            _isActive = false;
        }

        private void CenterOnObject()
        {
            _isActive = true;
            var newPosition = target.position;
            newPosition.y += yOffset;
            if (lockZ) newPosition.z = _initialZ;
            if (lockY) newPosition.y = yOffset;
            StartCoroutine(SmoothCameraTransition(newPosition));
        }

        private IEnumerator SmoothCameraTransition(Vector3 targetPosition)
        {
            if (!_mainCamera.gameObject.activeSelf)
                yield return null;

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

        public void OnNotify(GameEventData eventData)
        {
            StopAllCoroutines();
            _centerOnObject = false;

            // start a new, fast smooth transition
            transitionDuration = 2;
            StartCoroutine(SmoothCameraTransition(target.position));
        }
    }
}