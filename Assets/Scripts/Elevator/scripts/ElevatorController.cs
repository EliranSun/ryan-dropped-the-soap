using System.Collections;
using TMPro;
using UnityEngine;

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

        private int _currentFloor;
        private Vector3 _initLightPosition;
        private bool _isFloorMoving;
        private float _timeDiff;
        private float _timeSinceLastClick;

        private void Awake()
        {
            // making sure to notify this before any other event
            Notify(GameEvents.FloorChange, _currentFloor);
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

            if (floorText) floorText.text = floorData.currentFloorNumber.ToString();
        }

        private void Update()
        {
            _timeSinceLastClick += Time.deltaTime;

            if (_isFloorMoving)
                return;

            if (_currentFloor == int.Parse(floorText.text))
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
        }

        private void GoToFloor(int floorNumber)
        {
            StartCoroutine(Move(floorNumber));
            // StartCoroutine(MoveApartmentsGrid());
            shakeableCamera.Shake(Mathf.Abs(floorNumber - _currentFloor));
        }

        private IEnumerator Move(int floorNumber)
        {
            Notify(GameEvents.ElevatorMoving);

            elevatorAudioSource.clip = elevatorMovingSound;
            elevatorAudioSource.loop = true;
            elevatorAudioSource.Play();

            _isFloorMoving = true;

            while (_currentFloor != floorNumber)
            {
                yield return new WaitForSeconds(1);

                if (_currentFloor < floorNumber)
                    _currentFloor++;
                else
                    _currentFloor--;

                Notify(GameEvents.FloorChange, _currentFloor);
                floorText.text = $"{_currentFloor}";
                for (var i = 0; i < apartmentNumbers.Length; i++)
                {
                    var apt = apartmentNumbers[i];
                    apt.text = $"{_currentFloor}0{i + 1}";
                }
            }

            _isFloorMoving = false;

            elevatorAudioSource.clip = elevatorReachedFloorSound;
            elevatorAudioSource.loop = false;
            elevatorAudioSource.Play();

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
            int.TryParse(buttonNumberString, out var buttonNumber);

            UpdateFloor(buttonNumber);
        }
    }
}