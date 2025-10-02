using TMPro;
using UnityEngine;

namespace Elevator.scripts
{
    public class ElevatorCall : ObserverSubject
    {
        [SerializeField] private float minDistanceForCall = 4f;
        [SerializeField] private TextMeshPro currentElevatorFloorIndication;
        [SerializeField] private GameObject doors;
        [SerializeField] private ElevatorController controller;
        [SerializeField] private Transform playerTransform;
        private bool _areDoorsOpen;

        private void Update()
        {
            var distanceToElevator = Vector3.Distance(playerTransform.position, transform.position);

            if (Input.GetKeyDown(KeyCode.X) && distanceToElevator <= minDistanceForCall)
            {
                if (_areDoorsOpen) Notify(GameEvents.EnterElevator);
                else controller.CallElevator(transform.position.y);
            }
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.FloorChange)
            {
                var floorNumber = (int)eventData.Data;
                currentElevatorFloorIndication.text = floorNumber.ToString();
            }

            if (eventData.Name == GameEvents.ElevatorReachedFloor)
            {
                var elevatorYPosition = (float)eventData.Data;
                var distanceToSelf = Mathf.Abs(elevatorYPosition - transform.position.y);
                if (distanceToSelf <= 1)
                {
                    if (doors) doors.gameObject.SetActive(false);
                    _areDoorsOpen = true;
                }
            }
        }
    }
}