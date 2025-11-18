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
        [SerializeField] private bool lockX;
        [SerializeField] private int centerOnObjectDelay;
        [SerializeField] private int transitionDuration = 5;
        [SerializeField] private bool catchUpMode;
        [SerializeField] private float catchUpSpeed = 5f;
        private bool _centerOnObject;
        private float _initialZ;
        private bool _isActive;
        private bool _isCatchingUp;
        private Vector3 _lastTargetPosition;
        private float _lockedX;

        private Camera _mainCamera;

        // For SmoothDamp
        private Vector3 _velocity = Vector3.zero;

        private void Start()
        {
            _mainCamera = GetComponent<Camera>();
            _initialZ = transform.position.z;
            _lockedX = transform.position.x;
            _lastTargetPosition = target ? target.position : Vector3.zero;

            if (centerOnObjectDelay > 0) Invoke(nameof(CenterOnObject), centerOnObjectDelay);
        }

        private void Update()
        {
            if (!_isActive || !target) return;

            if (catchUpMode)
            {
                var targetPos = target.position;
                targetPos.y += yOffset;
                if (lockX) targetPos.x = _lockedX;
                else if (!_centerOnObject) targetPos.x += xOffset;
                if (lockZ) targetPos.z = _initialZ;
                if (lockY) targetPos.y = yOffset;

                // Check if target is moving
                var isMoving = (target.position - _lastTargetPosition).sqrMagnitude > 0.001f;

                if (isMoving)
                {
                    // Camera follows with lag
                    transform.position =
                        Vector3.SmoothDamp(transform.position, targetPos, ref _velocity, 1f / catchUpSpeed);
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
            if (lockX) newPosition.x = _lockedX;
            else if (!_centerOnObject) newPosition.x += xOffset;

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
            if (lockX) newPosition.x = _lockedX;
            else if (!_centerOnObject) newPosition.x += xOffset;
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
                if (lockX) currentTargetPosition.x = _lockedX;
                else if (!_centerOnObject) currentTargetPosition.x += xOffset;
                if (lockZ) currentTargetPosition.z = _initialZ;
                if (lockY) currentTargetPosition.y = yOffset;

                elapsedTime += Time.deltaTime;
                var t = elapsedTime / transitionDuration;
                // var t = elapsedTime / 1;
                // var t = 2f;
                _mainCamera.gameObject.transform.position = Vector3.Lerp(startPosition, currentTargetPosition, t);
                yield return null;
            }

            // Final position should be the current target position
            var finalPosition = target.position;
            finalPosition.y += yOffset;
            if (lockX) finalPosition.x = _lockedX;
            else if (!_centerOnObject) finalPosition.x += xOffset;
            if (lockZ) finalPosition.z = _initialZ;
            if (lockY) finalPosition.y = yOffset;

            _mainCamera.gameObject.transform.position = finalPosition;
            _centerOnObject = true;
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.LineNarrationEnd)
            {
                StopAllCoroutines();
                _centerOnObject = false;

                // start a new, fast smooth transition
                transitionDuration = 2;
                StartCoroutine(SmoothCameraTransition(target.position));
            }
        }

        public void LockX(float xPosition)
        {
            _lockedX = xPosition;
            lockX = true;
            // TODO: When is active, object follow lag behind on elevator change
            _isActive = false;
        }

        public void UnlockX()
        {
            lockX = false;
            _isActive = true;
        }
    }
}