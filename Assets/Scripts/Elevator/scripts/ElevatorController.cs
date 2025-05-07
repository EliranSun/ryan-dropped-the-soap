using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Elevator.scripts
{
    public class ElevatorController : ObserverSubject
    {
        [SerializeField] private AudioSource elevatorAudioSource;
        [SerializeField] private AudioClip elevatorMovingSound;

        [SerializeField] private AudioClip elevatorReachedFloorSound;

        // [SerializeField] private Transform apartmentsGridTransform;
        [SerializeField] private ElevatorShake shakeableCamera;
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshPro floorText;
        [SerializeField] private TextMeshPro desiredFloorText;
        [SerializeField] private TextMeshPro[] apartmentNumbers;
        [SerializeField] private GameObject[] panelNumbers;
        [SerializeField] private GameObject shaftLight;
        [SerializeField] private float debounce = 3f;
        [SerializeField] private float lightLoop = 3f;
        [SerializeField] private float apartmentsPanelMoveSpeed = 1;
        [SerializeField] private Common.FloorData floorData;

        private int _desiredFloor;
        private Vector3 _initLightPosition;
        private bool _isFloorMoving;
        private float _timeDiff;
        private float _timeSinceLastClick;

        private void Awake()
        {
            // making sure to notify this before any other event
            Notify(GameEvents.FloorChange, floorData.elevatorFloorNumber);
        }

        private void Start()
        {
            Debug.Log("ElevatorController Start method called");
            // _initLightPosition = shaftLight.transform.position;
            //
            // var panelChildren = panel.transform.childCount;
            // for (var i = 0; i < panelChildren; i++)
            // {
            //     var child = panel.transform.GetChild(i);
            //     var button = child.GetComponent<Button>();
            //     button.onClick.AddListener(() => UpdateFloor(button));
            // }
            //
            // StartCoroutine(ControlShaftLight());

            if (floorText) floorText.text = floorData.elevatorFloorNumber.ToString();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X) && !_isFloorMoving)
            {
                floorData.playerExitElevator = true;
                SceneManager.LoadScene("3. building scene");
            }

            _timeSinceLastClick += Time.deltaTime;

            if (_isFloorMoving)
                return;

            if (floorData.elevatorFloorNumber == int.Parse(floorText.text))
                return;

            if (_timeSinceLastClick == 0)
                return;

            if (_timeSinceLastClick - _timeDiff < debounce)
                return;

            // var floor = int.Parse(floorText.text);
            // GoToFloor(floor);
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.ElevatorButtonPress)
            {
                print("Elevator Button Press event " + eventData.Data);
                var floor = (int)eventData.Data;
                GoToFloor(floor);
            }
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

        private void GoToFloor(int floorNumber)
        {
            StartCoroutine(Move(floorNumber));
            // StartCoroutine(MoveApartmentsGrid());
            print("Desired floor - " + floorNumber + "; current floor - " + floorData.elevatorFloorNumber);
            shakeableCamera.Shake(Mathf.Abs(floorNumber - floorData.elevatorFloorNumber));
            shaftLight.SetActive(false);
        }

        private IEnumerator Move(int floorNumber)
        {
            Notify(GameEvents.ElevatorMoving);

            if (elevatorAudioSource)
            {
                elevatorAudioSource.clip = elevatorMovingSound;
                elevatorAudioSource.loop = true;
                elevatorAudioSource.Play();
            }

            _isFloorMoving = true;

            while (floorData.elevatorFloorNumber != floorNumber)
            {
                yield return new WaitForSeconds(1);

                if (floorData.elevatorFloorNumber < floorNumber)
                {
                    floorData.elevatorFloorNumber++;
                    floorData.playerFloorNumber++;
                }
                else
                {
                    floorData.elevatorFloorNumber--;
                    floorData.playerFloorNumber--;
                }

                Notify(GameEvents.FloorChange, floorData.elevatorFloorNumber);
                floorText.text = $"{floorData.elevatorFloorNumber}";
                for (var i = 0; i < apartmentNumbers.Length; i++)
                {
                    var apt = apartmentNumbers[i];
                    apt.text = $"{floorData.elevatorFloorNumber}0{i + 1}";
                }
            }

            _isFloorMoving = false;
            shaftLight.SetActive(true);

            if (elevatorAudioSource)
            {
                elevatorAudioSource.clip = elevatorReachedFloorSound;
                elevatorAudioSource.loop = false;
                elevatorAudioSource.Play();
            }

            Invoke(nameof(NotifyElevatorReachedFloor), 2);
        }

        private void NotifyElevatorReachedFloor()
        {
            Notify(GameEvents.ElevatorReachedFloor);
        }

        // private IEnumerator MoveApartmentsGrid()
        // {
        //     while (_isFloorMoving)
        //     {
        //         var time = Time.deltaTime;
        //         var pointA = apartmentsGridTransform.localPosition;
        //         var pointB = apartmentsGridTransform.localPosition;
        //         pointB.y -= Time.deltaTime * apartmentsPanelMoveSpeed;
        //
        //         apartmentsGridTransform.localPosition = Vector3.Lerp(pointA, pointB, Time.deltaTime);
        //
        //         yield return new WaitForEndOfFrame();
        //     }
        // }

        private IEnumerator ControlShaftLight()
        {
            while (true)
            {
                yield return new WaitForSeconds(lightLoop);
                shaftLight.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
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