using UnityEngine;

namespace Elevator.scripts
{
    public class FloorController : MonoBehaviour
    {
        [SerializeField] private GameObject doors;
        [SerializeField] private GameObject floorSprite;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private Transform elevatorTransform;
        [SerializeField] private SpriteRenderer elevatorSprite;
        private bool _isElevatorReachedFloor;

        private void Start()
        {
            floorSprite.SetActive(false);
            doors.SetActive(false);
            // FadeElevatorBasedOnPlayerProximity();
        }

        private void Update()
        {
            if (!_isElevatorReachedFloor)
                return;

            FadeElevatorBasedOnPlayerProximity();
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.name == GameEvents.ElevatorMoving)
            {
                _isElevatorReachedFloor = false;
                floorSprite.SetActive(false);
                doors.SetActive(true);
            }

            if (eventData.name == GameEvents.ElevatorReachedFloor)
            {
                _isElevatorReachedFloor = true;
                floorSprite.SetActive(true);
                doors.SetActive(false);
            }

            // if (eventData.name == GameEvents.PlayerOutsideBuilding)
            // {
            //     floorSprite.SetActive(false);
            //     doors.SetActive(false);
            //     FadeElevatorBasedOnPlayerProximity();
            // }
        }

        private void FadeElevatorBasedOnPlayerProximity()
        {
            var distance = Vector2.Distance(playerTransform.position, elevatorTransform.position);

            if (distance < 2.5f)
            {
                elevatorSprite.color = new Color(1, 1, 1, 1);
                return;
            }

            var alpha = 1 - distance / 5;

            elevatorSprite.color = new Color(1, 1, 1, alpha);
        }
    }
}