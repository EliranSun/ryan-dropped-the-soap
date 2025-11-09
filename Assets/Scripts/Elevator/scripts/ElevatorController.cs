using System.Collections;
using UnityEngine;

namespace Elevator.scripts
{
    public class ElevatorController : ObserverSubject
    {
        [Header("Essentials")] [SerializeField]
        private GameObject elevator;

        [SerializeField] private GameObject exit;
        [SerializeField] public float floorHeight;
        [SerializeField] public int currentFloor;
        public int desiredFloor;
        public bool isFloorMoving;

        [Header("Sound")] [SerializeField] private AudioSource elevatorAudioSource;
        [SerializeField] private AudioClip elevatorMovingSound;
        [SerializeField] private AudioClip elevatorReachedFloorSound;

        [Header("Player interaction")] [SerializeField]
        private ElevatorButtonsController elevatorButtonsController;

        [Header("Configuration")] [SerializeField]
        private bool restoreElevatorFloor = true;

        [SerializeField] private float elevatorSpeed = 0.1f;
        [SerializeField] private float minYToStop;

        [Header("Animation")] [SerializeField] private GameObject shaftLight;

        [SerializeField] private float lightLoop = 3f;
        [SerializeField] private ElevatorShake shakeableCamera;

        private Rigidbody2D _elevatorRigidbody;
        private bool _hasReachedYTarget = true; // Track if we've reached the yTarget to prevent multiple notifications
        private Vector3 _initLightPosition;
        private float _yTarget;

        private void Start()
        {
            // Get and configure Rigidbody2D
            _elevatorRigidbody = elevator.GetComponent<Rigidbody2D>();
            _elevatorRigidbody.bodyType = RigidbodyType2D.Kinematic;
            _elevatorRigidbody.gravityScale = 0f;

            var storedCurrentFloor = PlayerPrefs.GetInt("currentFloor");
            if (storedCurrentFloor != 0 && restoreElevatorFloor)
                currentFloor = storedCurrentFloor;


            // Set initial position using Rigidbody2D
            var initialPosition = new Vector2(
                elevator.transform.position.x,
                CalculateElevatorYPosition(currentFloor)
            );
            _elevatorRigidbody.position = initialPosition;
            _yTarget = initialPosition.y;

            Notify(GameEvents.FloorChange, currentFloor);
        }

        // private void Update()
        // {
        //     if (Input.GetKeyDown(KeyCode.X) && !isFloorMoving)
        //         // if (floorData) floorData.playerExitElevator = true;
        //         Notify(GameEvents.ChangePlayerLocation, Location.Hallway);
        // }

        private void Update()
        {
            if (Mathf.Abs(_yTarget - _elevatorRigidbody.position.y) > minYToStop)
            {
                var direction = _yTarget > _elevatorRigidbody.position.y ? Vector2.up : Vector2.down;
                var newPosition = _elevatorRigidbody.position + direction * (Time.fixedDeltaTime * elevatorSpeed);
                _elevatorRigidbody.MovePosition(newPosition);
                print($"Elevator moving... {newPosition}");

                // Update floor number based on current position
                UpdateFloorBasedOnPosition();

                // Mark that we're moving towards target
                _hasReachedYTarget = false;
            }
            else if (!_hasReachedYTarget)
            {
                print("Elevator has reached the yTarget destination");
                _hasReachedYTarget = true;
                OnReachFloor();
            }
        }

        private float CalculateElevatorYPosition(int floorNumber)
        {
            // Assuming floor 0 is at Y position 0, calculate position for current floor
            // return floorNumber * floorHeight + elevator.transform.localScale.y / 2f;
            return floorNumber * floorHeight + 5;
        }

        /// <summary>
        ///     Updates the current floor number based on the elevator's Y position
        /// </summary>
        private void UpdateFloorBasedOnPosition()
        {
            // Calculate which floor we're currently on based on Y position
            // Assuming floor 0 is at Y position 0, each floor is floorHeight units apart
            var calculatedFloor = Mathf.RoundToInt(
                (elevator.transform.position.y - elevator.transform.localScale.y / 2f) / floorHeight);

            // Only update if the floor has actually changed
            if (calculatedFloor != currentFloor)
            {
                currentFloor = calculatedFloor;
                PlayerPrefs.SetInt("currentFloor", currentFloor);

                // Update floor display
                if (elevatorButtonsController && elevatorButtonsController.floorText)
                    elevatorButtonsController.floorText.text = currentFloor.ToString();

                // Notify other systems of floor change
                Notify(GameEvents.FloorChange, currentFloor);

                // Optional: Add some feedback when floor changes
                Debug.Log($"Elevator reached floor: {currentFloor}");
            }
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.FloorChange)
            {
                var floorNumber = (int)eventData.Data;
                currentFloor = floorNumber;
                PlayerPrefs.SetInt("currentFloor", currentFloor);

                Notify(GameEvents.FloorChange, currentFloor);

                if (elevatorButtonsController.floorText)
                    elevatorButtonsController.floorText.text = $"{currentFloor}";
            }


            if (eventData.Name == GameEvents.ElevatorButtonPress)
            {
                var floor = (int)eventData.Data;
                GoToFloor(floor);
            }

            if (eventData.Name == GameEvents.StopElevator)
                StopElevator();

            if (eventData.Name == GameEvents.ResumeElevator)
                GoToFloor(desiredFloor);
        }


        public void CallElevator(float yPosition)
        {
            exit.SetActive(false);
            print($"Should go to Y {yPosition}. Current Y: {elevator.transform.position.y}");
            _yTarget = yPosition; // height to reach floor as this is the middle of the floor
            _hasReachedYTarget = false; // Reset flag when setting new target
        }

        public void GoToFloor(int floorNumber)
        {
            desiredFloor = floorNumber;
            CallElevator(CalculateElevatorYPosition(desiredFloor));

            if (isFloorMoving)
            {
                // If we are already moving, update the shake amount for the new destination.
                shakeableCamera.Shake(Mathf.Abs(floorNumber - currentFloor));
                return;
            }

            StartCoroutine(InsideElevatorMove());
            // StartCoroutine(MoveApartmentsGrid());
            shakeableCamera.Shake(Mathf.Abs(floorNumber - currentFloor));
            shaftLight.SetActive(false);
        }

        private IEnumerator InsideElevatorMove()
        {
            Notify(GameEvents.ElevatorMoving);

            if (elevatorAudioSource)
            {
                elevatorAudioSource.clip = elevatorMovingSound;
                elevatorAudioSource.loop = true;
                elevatorAudioSource.Play();
            }

            isFloorMoving = true;

            while (currentFloor != desiredFloor)
                yield return new WaitForSeconds(0.5f);

            StopElevator();
            Invoke(nameof(OnReachFloor), 2);
        }

        private void StopElevator()
        {
            StopAllCoroutines();

            isFloorMoving = false;
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

        private void OnReachFloor()
        {
            exit.SetActive(true);
            Debug.Log($"OnReachFloor reached destination: {_yTarget}");
            Notify(GameEvents.ElevatorReachedFloor, _yTarget);
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
    }
}