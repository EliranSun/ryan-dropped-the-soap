using UnityEngine;
using UnityEngine.EventSystems;

namespace Elevator.scripts
{
    public class MouseScroll : MonoBehaviour
    {
        [SerializeField] private float scrollSpeed = 10f;
        [SerializeField] private float minScrollLimit;
        [SerializeField] private float maxScrollLimit = 12f;
        private bool _isElevatorMoving;
        private float _originalScrollYPosition;

        private void Start()
        {
            _originalScrollYPosition = transform.position.y;
        }

        private void Update()
        {
            if (_isElevatorMoving || EventSystem.current.IsPointerOverGameObject())
                return;

            var scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll == 0)
                return;

            var newPosition = transform.position;
            newPosition.y += scroll * scrollSpeed;
            newPosition.y = Mathf.Clamp(newPosition.y, minScrollLimit, maxScrollLimit);
            transform.position = newPosition;
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.name == GameEvents.ElevatorMoving)
            {
                _isElevatorMoving = true;
                var newPosition = transform.position;
                newPosition.y = _originalScrollYPosition;
                transform.position = newPosition;
            }
        }
    }
}