using TMPro;
using UnityEngine;

namespace Elevator.scripts
{
    public class ElevatorCall : MonoBehaviour
    {
        [SerializeField] private int floorNumber;
        [SerializeField] private float minDistanceForCall = 4f;
        [SerializeField] private TextMeshPro currentElevatorFloorIndication;
        [SerializeField] private GameObject doors;
        [SerializeField] private ElevatorController controller;
        [SerializeField] private Transform playerTransform;
        private bool _areDoorsOpen;


        private void Update()
        {
            var distanceToElevator = Vector3.Distance(
                playerTransform.position,
                transform.position
            );

            if (Input.GetKeyDown(KeyCode.X) && distanceToElevator <= minDistanceForCall)
                if (!_areDoorsOpen)
                    controller.CallElevator(floorNumber);
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.FloorChange)
            {
                var nextFloorNumber = (int)eventData.Data;
                print($"{gameObject.name} Current floor number {nextFloorNumber}");
                gameObject.tag = "Elevator Doors";
                currentElevatorFloorIndication.text = nextFloorNumber.ToString();
                ChangeDoorsState(nextFloorNumber);
            }

            if (eventData.Name == GameEvents.ElevatorReachedFloor)
            {
                var elevatorFloor = (int)eventData.Data;
                print($"Elevator reached floor {elevatorFloor}. Reporting from floor {floorNumber}");
                ChangeDoorsState(elevatorFloor);
            }
        }

        public void UpdateFloorNumber(int newFloorNumber)
        {
            floorNumber = newFloorNumber;
        }

        private void ChangeDoorsState(int elevatorFloorNumber)
        {
            var isSameFloor = elevatorFloorNumber == floorNumber;
            if (doors) doors.gameObject.SetActive(!isSameFloor);
            gameObject.tag = isSameFloor ? "Elevator Entrance" : "Elevator Doors";
            _areDoorsOpen = isSameFloor;
        }
    }
}