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
                elapsedTime += Time.deltaTime;
                var t = elapsedTime / transitionDuration;
                _mainCamera.gameObject.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                yield return null;
            }

            // Ensure we end at exactly the target position
            _mainCamera.gameObject.transform.position = targetPosition;
            _centerOnObject = true;
        }
    }
}