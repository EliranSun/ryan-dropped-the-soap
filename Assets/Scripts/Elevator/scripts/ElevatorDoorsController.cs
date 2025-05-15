using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Elevator.scripts
{
    [RequireComponent(typeof(Collider2D))]
    public class ElevatorDoorsController : MonoBehaviour
    {
        [SerializeField] private Transform doorsOpenPosition;
        [SerializeField] private Transform leftDoorTransform;
        [SerializeField] private Transform rightDoorTransform;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float distanceToOpen = 1.5f;
        [SerializeField] private float timeToOpen = 1.5f;
        [SerializeField] private float delayBeforeOpening = 2f;
        [SerializeField] private float delayBeforeClosing = 2f;
        [SerializeField] private TextMeshPro elevatorCurrentFloorNumberUI;
        [SerializeField] private FloorController floorController;
        [SerializeField] private Common.FloorData floorData;

        private float _closeLerpTime;
        private Collider2D _collider2D;
        private bool _doorCloseCoroutineStarted;
        private bool _doorOpenCoroutineStarted;
        private bool _doorsAreOpen;
        private int _elevatorCurrentFloorNumber;
        private Vector3 _initialPositionLeft;
        private Vector3 _initialPositionRight;
        private float _openLerpTime;

        private void Start()
        {
            _collider2D = GetComponent<Collider2D>();

            if (rightDoorTransform) _initialPositionRight = rightDoorTransform.position;
            if (leftDoorTransform) _initialPositionLeft = leftDoorTransform.position;

            _doorsAreOpen = false;
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

        public void OnMouseDown()
        {
            if (_doorsAreOpen)
            {
                SceneManager.LoadScene("Scenes/inside elevator");
                return;
            }

            print("Call elevator to floor " + floorData.currentFloorNumber);
            StartCoroutine(floorController.CallElevator(this));
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

        public void SetElevatorCurrentFloorNumber(int floorNumber)
        {
            _elevatorCurrentFloorNumber = floorNumber;
            elevatorCurrentFloorNumberUI.text = _elevatorCurrentFloorNumber.ToString();
        }

        public void SetCurrentFloorNumber(int floorNumber)
        {
            floorData.currentFloorNumber = floorNumber;
        }

        public void OpenDoors()
        {
            StartCoroutine(OpenDoorsWithDelay());
        }
    }
}