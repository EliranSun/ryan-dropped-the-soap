using TMPro;
using UnityEngine;

namespace Elevator.scripts
{
    public class ElevatorCall : MonoBehaviour
    {
        [SerializeField] private float minDistanceForCall = 4f;
        [SerializeField] private TextMeshPro currentElevatorFloorIndication;
        private ElevatorController _controller;
        private Transform _playerTransform;

        private void Start()
        {
            var elevator = GameObject.FindGameObjectWithTag("Elevator");
            print($"Elevator found? {elevator.gameObject.name}");

            if (elevator) _controller = elevator.GetComponent<ElevatorController>();
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Update()
        {
            var distanceToElevator = Vector3.Distance(_playerTransform.position, transform.position);

            if (Input.GetKeyDown(KeyCode.X) && distanceToElevator < minDistanceForCall)
                _controller.CallElevator(transform.position.y);
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.FloorChange)
                currentElevatorFloorIndication.text = (string)eventData.Data;
        }
    }
}