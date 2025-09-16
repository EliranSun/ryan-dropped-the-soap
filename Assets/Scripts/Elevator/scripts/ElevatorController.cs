using System.Collections;
using Object.Scripts;
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
        [SerializeField] private int currentFloor;
        [SerializeField] private float floorHeight;

        private int _desiredFloor;
        private Vector3 _initLightPosition;
        private bool _isFloorMoving;
        private float _timeDiff;
        private float _timeSinceLastClick;
        private float yTarget;

        private void Start()
        {
            var storedCurrentFloor = PlayerPrefs.GetInt("currentFloor");
            if (storedCurrentFloor != 0) currentFloor = storedCurrentFloor;
            if (floorText) floorText.text = currentFloor.ToString();

            transform.position = new Vector3(transform.position.x, GetElevatorYPosition(), transform.position.z);
            yTarget = transform.position.y;
        }

        private void Update()
        {
            if (Mathf.Abs(yTarget - transform.position.y) > 1f)
            {
                var direction = yTarget > transform.position.y ? Vector3.up : Vector3.down;
                transform.Translate(direction * (Time.deltaTime * elevatorSpeed));

                // Update floor number based on current position
                UpdateFloorBasedOnPosition();
            }

            if (Input.GetKeyDown(KeyCode.X) && !_isFloorMoving)
                // if (floorData) floorData.playerExitElevator = true;
                Notify(GameEvents.ChangePlayerLocation, Location.Hallway);

            _timeSinceLastClick += Time.deltaTime;

            if (_isFloorMoving)
            {
                var y = (_desiredFloor < currentFloor ? -elevatorSpeed : elevatorSpeed) * Time.deltaTime;
                transform.Translate(0, y, 0);

                // Update floor number based on current position during movement
                UpdateFloorBasedOnPosition();
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

        private float GetElevatorYPosition()
        {
            // Assuming floor 0 is at Y position 0, calculate position for current floor
            return currentFloor * floorHeight + transform.localScale.y / 2f;
        }

        /// <summary>
        /// Updates the current floor number based on the elevator's Y position
        /// </summary>
        private void UpdateFloorBasedOnPosition()
        {
            // Calculate which floor we're currently on based on Y position
            // Assuming floor 0 is at Y position 0, each floor is floorHeight units apart
            int calculatedFloor = Mathf.RoundToInt((transform.position.y - transform.localScale.y / 2f) / floorHeight);

            // Only update if the floor has actually changed
            if (calculatedFloor != currentFloor)
            {
                currentFloor = calculatedFloor;
                PlayerPrefs.SetInt("currentFloor", currentFloor);

                // Update floor display
                if (floorText) floorText.text = currentFloor.ToString();

                // Update apartment numbers
                for (var i = 0; i < apartmentNumbers.Length; i++)
                {
                    var apt = apartmentNumbers[i];
                    apt.text = $"{currentFloor}0{i + 1}";
                }

                // Notify other systems of floor change
                Notify(GameEvents.FloorChange, currentFloor);

                // Optional: Add some feedback when floor changes
                Debug.Log($"Elevator reached floor: {currentFloor}");
            }
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.PlayerInteraction)
            {
                var objectName = (ObjectNames)eventData.Data;
                if (objectName == ObjectNames.Elevator) print("player called elevator" + transform.position);

                return;
            }

            if (eventData.Name == GameEvents.FloorChange)
            {
                var floorNumber = (int)eventData.Data;
                currentFloor = floorNumber;
                PlayerPrefs.SetInt("currentFloor", currentFloor);

                Notify(GameEvents.FloorChange, currentFloor);
                floorText.text = $"{currentFloor}";
                for (var i = 0; i < apartmentNumbers.Length; i++)
                {
                    var apt = apartmentNumbers[i];
                    apt.text = $"{currentFloor}0{i + 1}";
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

        public void CallElevator(float yPosition)
        {
            print("Should go to " + yPosition);
            yTarget = yPosition;
        }

        public void GoToFloor(int floorNumber)
        {
            _desiredFloor = floorNumber;

            if (_isFloorMoving)
            {
                // If we are already moving, just update the shake amount for the new destination.
                shakeableCamera.Shake(Mathf.Abs(floorNumber - currentFloor));
                return;
            }

            StartCoroutine(Move());
            // StartCoroutine(MoveApartmentsGrid());
            shakeableCamera.Shake(Mathf.Abs(floorNumber - currentFloor));
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

            while (currentFloor != _desiredFloor)
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

            print("Button number clicked: " + buttonNumber);
            UpdateFloor(buttonNumber);
        }
    }
}