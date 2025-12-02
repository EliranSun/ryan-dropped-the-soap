using System.Collections;
using Player;
using TMPro;
using UnityEngine;

namespace Elevator.scripts
{
    [RequireComponent(typeof(Collider2D))]
    public class ElevatorDoorsController : ObserverSubject
    {
        [SerializeField] private Transform leftDoorTransform;
        [SerializeField] private Transform rightDoorTransform;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float distanceToOpen = 1.5f;
        [SerializeField] private float timeToOpen = 1.5f;
        [SerializeField] private float delayBeforeOpening = 2f;
        [SerializeField] private float delayBeforeClosing = 2f;

        [SerializeField] private TextMeshPro elevatorCurrentFloorNumberUI;
        // private BuildingController _buildingController;

        private float _closeLerpTime;
        private bool _doorCloseCoroutineStarted;
        private bool _doorOpenCoroutineStarted;
        private bool _doorsAreOpen;
        private int _elevatorCurrentFloorNumber;
        private Vector3 _initialPositionLeft;
        private Vector3 _initialPositionRight;
        private float _openLerpTime;

        private void Start()
        {
            if (rightDoorTransform) _initialPositionRight = rightDoorTransform.position;
            if (leftDoorTransform) _initialPositionLeft = leftDoorTransform.position;

            _doorsAreOpen = false;

            // TODO: Better than hardcoded string
            // _buildingController = GameObject.Find("🏢 Building controller").GetComponent<BuildingController>();

            // Add player as observer
            var playerStatesController = FindFirstObjectByType<PlayerStatesController>();
            if (playerStatesController != null) observers.AddListener(playerStatesController.OnNotify);

            // SetElevatorCurrentFloorNumber(_buildingController.elevatorFloorNumber.ToString());
        }

        private void Update()
        {
            if (!playerTransform) return;

            var distance = Vector3.Distance(playerTransform.position, transform.position);

            if (distance < distanceToOpen && !_doorsAreOpen && !_doorOpenCoroutineStarted)
            {
                StartCoroutine(OpenDoorsWithDelay());
                _doorOpenCoroutineStarted = true;
            }

            if (distance >= distanceToOpen && _doorsAreOpen && !_doorCloseCoroutineStarted)
            {
                StartCoroutine(CloseDoorsWithDelay());
                _doorCloseCoroutineStarted = false;
            }
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name != GameEvents.ClickOnItem) return;
            if (eventData.Data is not string itemName) return;
            if (!itemName.ToLower().Contains("elevator")) return;

            if (_doorsAreOpen)
                // FIXME: This won't work as well since player scene is determined via PlayerStatesController
                // SceneManager.LoadScene("inside elevator");
                Notify(GameEvents.ChangePlayerLocation, Location.Elevator);

            // StartCoroutine(_buildingController.CallElevator(this));
        }

        private IEnumerator OpenDoorsWithDelay()
        {
            yield return new WaitForSeconds(delayBeforeOpening);

            _openLerpTime = 0;
            _closeLerpTime = 0;

            var rightFinalPosition = _initialPositionRight;
            var leftFinalPosition = _initialPositionLeft;

            rightFinalPosition.x = _initialPositionRight.x + 2;
            leftFinalPosition.x = _initialPositionLeft.x - 2;

            while (rightDoorTransform.position != rightFinalPosition)
            {
                _openLerpTime += Time.deltaTime / timeToOpen;
                rightDoorTransform.position =
                    Vector3.Lerp(rightDoorTransform.position, rightFinalPosition, _openLerpTime);
                leftDoorTransform.position =
                    Vector3.Lerp(rightDoorTransform.position, leftFinalPosition, _openLerpTime);
                yield return null;
            }

            _doorsAreOpen = true;
            _doorOpenCoroutineStarted = false;
            _doorCloseCoroutineStarted = false;
        }

        private IEnumerator CloseDoorsWithDelay()
        {
            yield return new WaitForSeconds(delayBeforeClosing);
            _openLerpTime = 0;
            _closeLerpTime = 0;

            while (rightDoorTransform.position != _initialPositionRight)
            {
                _closeLerpTime += Time.deltaTime / timeToOpen;
                rightDoorTransform.position =
                    Vector3.Lerp(rightDoorTransform.position, _initialPositionRight, _closeLerpTime);
                leftDoorTransform.position =
                    Vector3.Lerp(leftDoorTransform.position, _initialPositionLeft, _closeLerpTime);
                yield return null;
            }

            _doorsAreOpen = false;
            _doorCloseCoroutineStarted = false;
            _doorOpenCoroutineStarted = false;
        }

        public void SetElevatorCurrentFloorNumber(string floorNumber)
        {
            elevatorCurrentFloorNumberUI.text = floorNumber;
        }

        public void OpenDoors()
        {
            StartCoroutine(OpenDoorsWithDelay());
        }
    }
}