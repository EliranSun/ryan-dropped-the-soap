using System.Collections;
using common.scripts;
using TMPro;
using UnityEngine;

namespace Elevator.scripts
{
    public class FloorController : MonoBehaviour
    {
        [SerializeField] private DoorController[] hallwayDoors;

        [SerializeField] private TextMeshPro[] floorNumbers;
        [SerializeField] private ElevatorDoorsController[] elevators;

        // [FormerlySerializedAs("doors")] 
        //
        // [SerializeField] private SpriteRenderer outsideElevatorSprite;
        // [SerializeField] private GameObject[] outsideElevatorObjects;
        // [SerializeField] private GameObject[] insideElevatorObjects;
        // [SerializeField] private GameObject floorSprite;
        // [SerializeField] private Transform playerTransform;
        // [SerializeField] private Transform elevatorTransform;
        // [SerializeField] private SpriteRenderer elevatorSprite;
        // private bool _isElevatorReachedFloor;
        private readonly int _floorNumber = 20;
        private int _elevatorCurrentFloorNumber = 15;

        private void Start()
        {
            // floorSprite.SetActive(false);
            // floorDoors.SetActive(false);
            // FadeElevatorBasedOnPlayerProximity();

            for (var i = 0; i < floorNumbers.Length; i++)
                floorNumbers[i].text = (_floorNumber - i).ToString();

            for (var i = 0; i < elevators.Length; i++)
            {
                elevators[i].SetElevatorCurrentFloorNumber(_elevatorCurrentFloorNumber);
                elevators[i].SetCurrentFloorNumber(_floorNumber - i);
            }
        }

        private void Update()
        {
            // if (!_isElevatorReachedFloor)
            //     return;
            //
            // FadeElevatorBasedOnPlayerProximity();
        }

        public void OnNotify(GameEventData eventData)
        {
            // switch (eventData.Name)
            // {
            //     case GameEvents.ElevatorMoving:
            //         _isElevatorReachedFloor = false;
            //         floorSprite.SetActive(false);
            //         floorDoors.SetActive(true);
            //         break;
            //
            //     case GameEvents.ElevatorReachedFloor:
            //         _isElevatorReachedFloor = true;
            //         floorSprite.SetActive(true);
            //         floorDoors.SetActive(false);
            //         break;
            //
            //     case GameEvents.PlayerInsideElevator:
            //         floorSprite.SetActive(false);
            //         floorDoors.SetActive(true);
            //         outsideElevatorSprite.color = new Color(1, 1, 1, 0.2f);
            //         foreach (var outsideElevatorObject in outsideElevatorObjects)
            //             outsideElevatorObject.SetActive(false);
            //
            //         foreach (var insideElevatorObject in insideElevatorObjects)
            //             insideElevatorObject.SetActive(true);
            //         break;
            //
            //     case GameEvents.ExitElevatorToFloors:
            //         floorSprite.SetActive(true);
            //         floorDoors.SetActive(false);
            //         outsideElevatorSprite.color = new Color(1, 1, 1, 1);
            //         foreach (var outsideElevatorObject in outsideElevatorObjects)
            //             outsideElevatorObject.SetActive(true);
            //
            //         foreach (var insideElevatorObject in insideElevatorObjects)
            //             insideElevatorObject.SetActive(false);
            //         break;
            //
            //     case GameEvents.ExitElevatorToLobby:
            //         floorSprite.SetActive(false);
            //         floorDoors.SetActive(true);
            //         outsideElevatorSprite.color = new Color(1, 1, 1, 1);
            //         foreach (var outsideElevatorObject in outsideElevatorObjects)
            //             outsideElevatorObject.SetActive(true);
            //
            //         foreach (var insideElevatorObject in insideElevatorObjects)
            //             insideElevatorObject.SetActive(false);
            //         break;
            // }
        }

        // private void FadeElevatorBasedOnPlayerProximity()
        // {
        //     var distance = Vector2.Distance(playerTransform.position, elevatorTransform.position);
        //
        //     if (distance < 2.5f)
        //     {
        //         elevatorSprite.color = new Color(1, 1, 1, 1);
        //         return;
        //     }
        //
        //     var alpha = 1 - distance / 5;
        //
        //     elevatorSprite.color = new Color(1, 1, 1, alpha);
        // }

        public IEnumerator CallElevator(ElevatorDoorsController requestedElevator)
        {
            while (true)
            {
                if (_elevatorCurrentFloorNumber == requestedElevator.currentFloorNumber)
                {
                    requestedElevator.OpenDoors();
                    break;
                }

                if (_elevatorCurrentFloorNumber < requestedElevator.currentFloorNumber) _elevatorCurrentFloorNumber++;
                if (_elevatorCurrentFloorNumber > requestedElevator.currentFloorNumber) _elevatorCurrentFloorNumber--;

                foreach (var elevator in elevators)
                    elevator.SetElevatorCurrentFloorNumber(_elevatorCurrentFloorNumber);

                yield return new WaitForSeconds(1f);
            }
        }
    }
}