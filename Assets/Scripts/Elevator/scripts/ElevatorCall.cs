using TMPro;
using UnityEngine;

namespace Elevator.scripts
{
    public class ElevatorCall : ObserverSubject
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
                var floorNumber = (int)eventData.Data;
                gameObject.tag = "Elevator Doors";
                currentElevatorFloorIndication.text = floorNumber.ToString();
            }

            if (eventData.Name == GameEvents.ElevatorReachedFloor)
            {
                var elevatorFloor = (int)eventData.Data;
                if (elevatorFloor == floorNumber)
                {
                    if (doors) doors.gameObject.SetActive(false);
                    gameObject.tag = "Elevator Entrance";
                    _areDoorsOpen = true;
                }
            }
        }

        public void UpdateFloorNumber(int newFloorNumber)
        {
            floorNumber = newFloorNumber;
        }
    }
}