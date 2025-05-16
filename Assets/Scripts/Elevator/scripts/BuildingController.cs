using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        [SerializeField] private Transform playerTransform;

        // [SerializeField] private TextMeshPro[] floorNumbers;
        // [SerializeField] private ElevatorDoorsController[] elevators;
        [SerializeField] private GameObject charlotte;
        [SerializeField] private GameObject zeke;
        [SerializeField] private GameObject stacy;

        private readonly List<GameObject> _floors = new();
        private int _floorIndex = -2;
        private int _highestActiveFloorIndex = -1; // Track the highest active floor index

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
            for (var i = _floorIndex; i < 3; i++)
                AddFloorAt(i, floorData.currentFloorNumber);

            // var isStrangerApartment = floorData.currentFloorNumber != floorData.playerApartmentFloor;
            //
            // charlotte.gameObject.SetActive(floorData.currentFloorNumber == floorData.charlotteInitFloorNumber);
            // zeke.gameObject.SetActive(floorData.currentFloorNumber == floorData.zekeFloorNumber);
            // stacy.gameObject.SetActive(floorData.currentFloorNumber == floorData.stacyFloorNumber);
            //
            // if (floorData.playerExitElevator || isStrangerApartment)
            // {
            //     var newPosition = playerTransform.position;
            //     newPosition.x = elevators[1].transform.position.x;
            //     playerTransform.position = newPosition;
            //     floorData.playerExitElevator = false;
            // }
            //
            // floorNumbers[0].text = (floorData.currentFloorNumber - 1).ToString();
            // floorNumbers[1].text = floorData.currentFloorNumber.ToString();
            // floorNumbers[2].text = (floorData.currentFloorNumber + 1).ToString();
            //
            // foreach (var elevator in elevators)
            //     elevator.SetElevatorCurrentFloorNumber(floorData.elevatorFloorNumber);
            //
            // for (var i = 0; i < hallwayDoors.Length; i++)
            //     hallwayDoors[i].SetDoorNumber($"{floorData.currentFloorNumber}{i}");
        }

        private void Update()
        {
            if (_floors.Count == 0) return;

            var nearLowestFloor = _floors[0];
            if (!nearLowestFloor) return;


            if (playerTransform.position.y <= nearLowestFloor.transform.position.y)
            {
                print("Should add floors below");
                AddFloorAt(_floorIndex - 1, floorData.currentFloorNumber);
            }
        }

        private void DisableFloorAt(int index)
        {
            if (index < 0 || index >= _floors.Count) return;

            var floor = _floors[index];
            if (floor == null) return;

            floor.SetActive(false);
        }

        private void AddFloorAt(int index, int currentFloorNumber)
        {
            var floor = Instantiate(floorPrefab, new Vector3(0, index * 10, 0), Quaternion.identity);
            floor.GetComponent<FloorController>().SetFloorNumber(currentFloorNumber + index);

            // Check if a floor already exists at this position before adding
            if (_floors.Any(f => Math.Abs(f.transform.position.y - floor.transform.position.y) < 0.1f))
                return;

            print("Adding floor at index" + _floors.Count);
            _floors.Insert(0, floor); // Add to beginning to maintain the lowest floor at index 0

            // Update the highest active floor index after insertion
            _highestActiveFloorIndex++;
            _floorIndex--;

            // If we have more than 5 active floors, disable the highest one
            if (_floors.Count > 5)
            {
                DisableFloorAt(_highestActiveFloorIndex);
                // Find the new highest active floor index
                for (var i = _highestActiveFloorIndex - 1; i >= 0; i--)
                    if (_floors[i] != null && _floors[i].activeSelf)
                    {
                        _highestActiveFloorIndex = i;
                        break;
                    }
            }
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