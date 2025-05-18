using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Elevator.scripts
{
    [Serializable]
    public class Apartment
    {
        public int number;
        public GameObject prefab;
    }

    public class BuildingController : MonoBehaviour
    {
        // [SerializeField] private DoorController[] hallwayDoors;

        [SerializeField] private GameObject floorPrefab;
        [SerializeField] private Common.FloorData floorData;
        [SerializeField] private Apartment[] significantApartments;
        [SerializeField] private GameObject floorsContainer;
        [SerializeField] private Transform playerTransform;

        // [SerializeField] private TextMeshPro[] floorNumbers;
        // [SerializeField] private ElevatorDoorsController[] elevators;
        [SerializeField] private GameObject charlotte;
        [SerializeField] private GameObject zeke;
        [SerializeField] private GameObject stacy;

        private readonly List<GameObject> _floors = new();
        private UnityEvent<GameEventData> _floorObservers;

        // [SerializeField] private SpriteRenderer outsideElevatorSprite;
        // [SerializeField] private GameObject[] outsideElevatorObjects;
        // [SerializeField] private GameObject[] insideElevatorObjects;
        // [SerializeField] private GameObject floorSprite;
        // [SerializeField] private Transform playerTransform;
        // [SerializeField] private Transform elevatorTransform;
        // [SerializeField] private SpriteRenderer elevatorSprite;
        // private bool _isElevatorReachedFloor;
        // private readonly int _floorNumber = 20;
        // private int _elevatorCurrentFloorNumber = 15;

        private void Start()
        {
            _floorObservers = new UnityEvent<GameEventData>();
            _floorObservers.AddListener(OnNotify);

            for (var i = 2; i > -2; i--)
                AddFloorAtBottom(floorData.currentFloorNumber + i, true);

            playerTransform.position = _floors
                .Find(floor => floor.name == $"Floor {floorData.currentFloorNumber}")
                .transform.position;
        }

        private enum FloorDirection
        {
            Top,
            Bottom
        }

        private void AddFloor(int floorNumber, FloorDirection direction, bool init = false)
        {
            float yPosition;
            var existingFloor = _floors.Find(f => f.name == $"Floor {floorNumber}");

            if (existingFloor)
            {
                existingFloor.SetActive(true);
                if (direction == FloorDirection.Bottom)
                    _floors.FindLast(f => f.activeSelf).SetActive(false);
                else
                    _floors.Find(f => f.activeSelf).SetActive(false);
                return;
            }

            if (direction == FloorDirection.Bottom)
                yPosition = _floors.Count > 0 ? _floors[0].transform.position.y - 10 : 0;
            else
                yPosition = _floors.Count > 0 ? _floors.Last().transform.position.y + 10 : 0;

            var floor = Instantiate(floorPrefab, new Vector3(0, yPosition, 0), Quaternion.identity);
            floor.transform.SetParent(floorsContainer.transform);
            floor.name = $"Floor {floorNumber}";
            floor.GetComponent<FloorController>().SetFloorNumber(floorNumber);
            floor.GetComponent<FloorController>().SetObserver(_floorObservers);

            if (direction == FloorDirection.Bottom)
                _floors.Insert(0, floor);
            else
                _floors.Add(floor);

            if (init) return;

            if (direction == FloorDirection.Bottom)
                _floors.FindLast(f => f.activeSelf).SetActive(false);
            else
                _floors.Find(f => f.activeSelf).SetActive(false);
        }

        private void AddFloorAtBottom(int floorNumber, bool init = false)
        {
            AddFloor(floorNumber, FloorDirection.Bottom, init);
        }

        private void AddFloorAtTop(int floorNumber)
        {
            AddFloor(floorNumber, FloorDirection.Top);
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.FloorChange)
            {
                print("FLOOR CHANGE!");
                var floorNumber = (int)eventData.Data;
                var playerFloorIndex = _floors.FindIndex(f => f.name == $"Floor {floorNumber}");

                if (playerFloorIndex >= _floors.Count - 1 ||
                    playerFloorIndex == _floors.FindLastIndex(f => f.activeSelf))
                    AddFloorAtTop(floorNumber + 1);

                else if (playerFloorIndex == 0 || playerFloorIndex == _floors.FindIndex(f => f.activeSelf))
                    AddFloorAtBottom(floorNumber - 1);
            }

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
                if (floorData.currentFloorNumber == floorData.elevatorFloorNumber)
                {
                    requestedElevator.OpenDoors();
                    break;
                }

                if (floorData.elevatorFloorNumber < floorData.currentFloorNumber) floorData.elevatorFloorNumber++;
                if (floorData.elevatorFloorNumber > floorData.currentFloorNumber) floorData.elevatorFloorNumber--;

                // foreach (var elevator in elevators)
                //     elevator.SetElevatorCurrentFloorNumber(floorData.elevatorFloorNumber);

                yield return new WaitForSeconds(1f);
            }
        }
    }
}