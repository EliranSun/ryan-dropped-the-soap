using System.Collections;
using Player;
using TMPro;
using UnityEngine;

namespace Elevator.scripts
{
    public class ElevatorController : ObserverSubject
    {
        [SerializeField] private AudioSource elevatorAudioSource;
        [SerializeField] private AudioClip elevatorMovingSound;
        [SerializeField] private AudioClip elevatorReachedFloorSound;
        [SerializeField] private ElevatorShake shakeableCamera;
        [SerializeField] private TextMeshPro floorText;
        [SerializeField] private TextMeshPro desiredFloorText;
        [SerializeField] private TextMeshPro[] apartmentNumbers;
        [SerializeField] private GameObject[] panelNumbers;
        [SerializeField] private GameObject shaftLight;
        [SerializeField] private float debounce = 3f;
        [SerializeField] private float lightLoop = 3f;
        [SerializeField] private float apartmentsPanelMoveSpeed = 1;
        [SerializeField] private float elevatorSpeed = 0.1f;

        private int _currentFloor;
        // [SerializeField] private FloorData floorData;

        private int _desiredFloor;
        private Vector3 _initLightPosition;
        private bool _isFloorMoving;
        private float _timeDiff;
        private float _timeSinceLastClick;

        private void Awake()
        {
            // if (floorData)
            //     Notify(GameEvents.FloorChange, floorData.elevatorFloorNumber);
        }

        private void Start()
        {
            if (floorText) floorText.text = _currentFloor.ToString();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X) && !_isFloorMoving)
                // if (floorData) floorData.playerExitElevator = true;
                Notify(GameEvents.ChangePlayerLocation, Location.Hallway);

            _timeSinceLastClick += Time.deltaTime;

            if (_isFloorMoving)
            {
                var y = (_desiredFloor < _currentFloor ? -elevatorSpeed : elevatorSpeed) * Time.deltaTime;
                transform.Translate(0, y, 0);
            }

            // if (floorData && floorData.elevatorFloorNumber == int.Parse(floorText.text))
            //     return;
            //
            // if (_timeSinceLastClick == 0)
            //     return;
            //
            // if (_timeSinceLastClick - _timeDiff < debounce)
            //     return;

            // var floor = int.Parse(floorText.text);
            // GoToFloor(floor);
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.FloorChange)
            {
                var floorNumber = (int)eventData.Data;
                _currentFloor = floorNumber;

                Notify(GameEvents.FloorChange, _currentFloor);
                floorText.text = $"{_currentFloor}";
                for (var i = 0; i < apartmentNumbers.Length; i++)
                {
                    var apt = apartmentNumbers[i];
                    apt.text = $"{_currentFloor}0{i + 1}";
                }
            }


            if (eventData.Name == GameEvents.ElevatorButtonPress)
            {
                var floor = (int)eventData.Data;
                GoToFloor(floor);
            }

            if (eventData.Name == GameEvents.StopElevator)
                StopElevator();

            if (eventData.Name == GameEvents.ResumeElevator)
                GoToFloor(_desiredFloor);
        }

        private void UpdateFloor(int floorNumber)
        {
            if (_isFloorMoving)
                return;

            _timeSinceLastClick = 0;
            _timeDiff = Time.deltaTime;

            desiredFloorText.text = desiredFloorText.text == "00" // init state
                ? $"{floorNumber}"
                : $"{desiredFloorText.text}{floorNumber}";


            _desiredFloor = int.Parse(desiredFloorText.text);
        }

        public void GoToFloor(int floorNumber)
        {
            _desiredFloor = floorNumber;

            if (_isFloorMoving)
            {
                // If we are already moving, just update the shake amount for the new destination.
                shakeableCamera.Shake(Mathf.Abs(floorNumber - _currentFloor));
                return;
            }

            StartCoroutine(Move());
            // StartCoroutine(MoveApartmentsGrid());
            shakeableCamera.Shake(Mathf.Abs(floorNumber - _currentFloor));
            shaftLight.SetActive(false);
        }

        private IEnumerator Move()
        {
            Notify(GameEvents.ElevatorMoving);

            if (elevatorAudioSource)
            {
                elevatorAudioSource.clip = elevatorMovingSound;
                elevatorAudioSource.loop = true;
                elevatorAudioSource.Play();
            }

            _isFloorMoving = true;

            while (_currentFloor != _desiredFloor)
                yield return new WaitForSeconds(0.5f);

            StopElevator();
            Invoke(nameof(NotifyElevatorReachedFloor), 2);
        }

        private void StopElevator()
        {
            StopAllCoroutines();

            _isFloorMoving = false;
            Invoke(nameof(OpenDoors), 1);

            if (elevatorAudioSource)
            {
                elevatorAudioSource.clip = elevatorReachedFloorSound;
                elevatorAudioSource.loop = false;
                elevatorAudioSource.Play();
            }

            shakeableCamera.Shake(0);
        }

        private void OpenDoors()
        {
            shaftLight.SetActive(true);
        }

        private void NotifyElevatorReachedFloor()
        {
            Notify(GameEvents.ElevatorReachedFloor);
        }

        private IEnumerator ControlShaftLight()
        {
            while (true)
            {
                yield return new WaitForSeconds(lightLoop);
                shaftLight.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, 0);
                shaftLight.transform.position = _initLightPosition;
            }
        }

        public void OnElevatorButtonClick(string buttonNumberString)
        {
            if (buttonNumberString == "go")
            {
                GoToFloor(_desiredFloor);
                return;
            }

            if (buttonNumberString == "reset")
            {
                desiredFloorText.text = "00";
                return;
            }

            int.TryParse(buttonNumberString, out var buttonNumber);

            UpdateFloor(buttonNumber);
        }
    }
}