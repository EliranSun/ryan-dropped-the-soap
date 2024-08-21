using UnityEngine;

namespace Elevator.scripts
{
    public class FloorController : MonoBehaviour
    {
        [SerializeField] private GameObject doors;
        [SerializeField] private SpriteRenderer outsideElevatorSprite;
        [SerializeField] private GameObject[] outsideElevatorObjects;
        [SerializeField] private GameObject[] insideElevatorObjects;
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
            print($"FloorController OnNotify: {eventData.name}");
            switch (eventData.name)
            {
                case GameEvents.ElevatorMoving:
                    _isElevatorReachedFloor = false;
                    floorSprite.SetActive(false);
                    doors.SetActive(true);
                    break;

                case GameEvents.ElevatorReachedFloor:
                    _isElevatorReachedFloor = true;
                    floorSprite.SetActive(true);
                    doors.SetActive(false);
                    break;

                case GameEvents.PlayerInsideElevator:
                    floorSprite.SetActive(false);
                    doors.SetActive(true);
                    outsideElevatorSprite.color = new Color(1, 1, 1, 0.2f);
                    foreach (var outsideElevatorObject in outsideElevatorObjects)
                        outsideElevatorObject.SetActive(false);

                    foreach (var insideElevatorObject in insideElevatorObjects)
                        insideElevatorObject.SetActive(true);
                    break;

                case GameEvents.PlayerExitElevator:
                    floorSprite.SetActive(false); // TODO depends to which end
                    doors.SetActive(false);
                    outsideElevatorSprite.color = new Color(1, 1, 1, 1);
                    foreach (var outsideElevatorObject in outsideElevatorObjects)
                        outsideElevatorObject.SetActive(true);

                    foreach (var insideElevatorObject in insideElevatorObjects)
                        insideElevatorObject.SetActive(false);
                    break;
            }
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