using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Elevator.scripts
{
    [Serializable]
    public enum Tenant
    {
        Zeke,
        Stacy
    }

    [Serializable]
    public class TenantApartment
    {
        public int apartmentNumber;
        public int currentApartmentNumber;
        public int floorNumber;
        public Tenant name;
        public GameObject tenantPrefab;
        public GameObject door;
        public GameObject apartmentContents;
    }


    public class BuildingController : ObserverSubject
    {
        [SerializeField] private GameObject floorPrefab;
        [SerializeField] private FloorData floorData;

        [SerializeField] public ApartmentController[] npcApartments;

        [SerializeField] private GameObject floorsContainer;
        [SerializeField] private Transform playerTransform;

        // [SerializeField] private GameObject charlotte;
        // [SerializeField] private GameObject zeke;
        // [SerializeField] private GameObject stacy;

        [SerializeField] public TenantApartment[] tenants;

        private readonly List<GameObject> _floors = new();
        private UnityEvent<GameEventData> _floorObservers;

        private void Awake()
        {
            _floorObservers = new UnityEvent<GameEventData>();
            _floorObservers.AddListener(OnNotify);

            for (var i = 2; i > -2; i--)
                AddFloorAtBottom(floorData.currentFloorNumber + i, true);


            PositionPlayerOnFloor();
            // PopulateApartments();
        }

        private void PositionPlayerOnFloor()
        {
            var playerPosition = _floors
                .Find(floor => floor.name == $"Floor {floorData.currentFloorNumber}")
                .transform.position;
            playerPosition.x = playerTransform.position.x;
            playerTransform.position = playerPosition;
        }

        // private void PopulateApartments()
        // {
        //     foreach (var npcApartment in npcApartments)
        //     {
        //         var floor = _floors.Find(f =>
        //             f.GetComponent<FloorController>().floorNumber == npcApartment.floorNumber);
        //
        //         if (floor == null) continue;
        //
        //         // var apartment = floor.GetComponent<FloorController>().apartments
        //         //     .First(apartment => apartment.apartmentNumber == npcApartment.apartmentNumber);
        //         //
        //         // if (apartment.prefab != null && !apartment.isPopulated)
        //         // {
        //         //     // Instantiate(significantApartment.prefab, apartment.prefab.transform);
        //         //
        //         //     // apartments should already be in scene, for making triggers and connections easier
        //         //     npcApartment.prefab.transform.position = apartment.prefab.transform.position;
        //         //     npcApartment.prefab.SetActive(true);
        //         //     apartment.isPopulated = true;
        //         // }
        //     }
        // }

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
            var floorController = floor.GetComponent<FloorController>();
            floorController.SetFloorNumber(floorNumber, this);
            floorController.SetObserver(_floorObservers);

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

                // PopulateApartments();
            }

            if (eventData.Name == GameEvents.KnockOnNpcDoor)
                // var npcDoorNumber = (int)eventData.Data;
                // var npcApartment =
                //     npcApartments.FirstOrDefault(apartment =>
                //         int.Parse($"{apartment.floorNumber}{apartment.apartmentNumber}") == npcDoorNumber);
                //
                // if (npcApartment != null && npcApartment.narrationLine != null)
                //     Notify(GameEvents.TriggerSpecificDialogLine, npcApartment.narrationLine);
                // notify again because doors are triggering original and is only connected to BuildingController
                // since they are dynamically created
                Notify(GameEvents.KnockOnNpcDoor, eventData.Data);
        }

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

                yield return new WaitForSeconds(1f);
            }
        }

        private enum FloorDirection
        {
            Top,
            Bottom
        }
    }
}