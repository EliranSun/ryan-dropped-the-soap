using TMPro;
using UnityEngine;

namespace Elevator.scripts
{
    public class ElevatorCall : ObserverSubject
    {
        [SerializeField] private float minDistanceForCall = 4f;
        [SerializeField] private TextMeshPro currentElevatorFloorIndication;
        [SerializeField] private GameObject doors;
        private bool _areDoorsOpen;
        private ElevatorController _controller;
        private Transform _playerTransform;

        private void Start()
        {
            var elevator = GameObject.FindGameObjectWithTag("Elevator");
            print($"Elevator found? {elevator.gameObject.name}");

            if (elevator) _controller = elevator.GetComponent<ElevatorController>();
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

            Notify(GameEvents.EnterElevator);
        }

        private void Update()
        {
            var distanceToElevator = Vector3.Distance(_playerTransform.position, transform.position);

            if (!Input.GetKeyDown(KeyCode.X) || !(distanceToElevator < minDistanceForCall))
                return;

            if (_areDoorsOpen) Notify(GameEvents.EnterElevator);
            else _controller.CallElevator(transform.position.y);
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