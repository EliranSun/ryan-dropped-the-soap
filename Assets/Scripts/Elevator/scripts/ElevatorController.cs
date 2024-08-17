using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Elevator.scripts
{
    public class ElevatorController : ObserverSubject
    {
        [SerializeField] private AudioSource elevatorAudioSource;
        [SerializeField] private AudioClip elevatorMovingSound;
        [SerializeField] private AudioClip elevatorReachedFloorSound;
        [SerializeField] private Transform apartmentsGridTransform;
        [SerializeField] private ElevatorShake shakeableCamera;
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshPro floorText;
        [SerializeField] private GameObject shaftLight;
        [SerializeField] private float debounce = 3f;
        [SerializeField] private float lightLoop = 3f;
        [SerializeField] private float apartmentsPanelMoveSpeed = 1;

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
            _initLightPosition = shaftLight.transform.position;

            var panelChildren = panel.transform.childCount;
            for (var i = 0; i < panelChildren; i++)
            {
                var child = panel.transform.GetChild(i);
                var button = child.GetComponent<Button>();
                button.onClick.AddListener(() => UpdateFloor(button));
            }

            StartCoroutine(ControlShaftLight());
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

            var floor = int.Parse(floorText.text);
            GoToFloor(floor);
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.name == GameEvents.ElevatorButtonPress)
            {
                print("Elevator Button Press event " + eventData.data);
                var floor = (int)eventData.data;
                GoToFloor(floor);
            }
        }

        private void UpdateFloor(Button button)
        {
            if (_isFloorMoving)
                return;

            _timeSinceLastClick = 0;
            _timeDiff = Time.deltaTime;

            var buttonName = button.name;
            var floor = int.Parse(buttonName);

            floorText.text = floorText.text == "00" // init state
                ? $"{floor}"
                : $"{floorText.text}{floor}";
        }

        private void GoToFloor(int floorNumber)
        {
            StartCoroutine(Move(floorNumber));
            StartCoroutine(MoveApartmentsGrid());
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

        private IEnumerator MoveApartmentsGrid()
        {
            while (_isFloorMoving)
            {
                var time = Time.deltaTime;
                var pointA = apartmentsGridTransform.localPosition;
                var pointB = apartmentsGridTransform.localPosition;
                pointB.y -= Time.deltaTime * apartmentsPanelMoveSpeed;

                apartmentsGridTransform.localPosition = Vector3.Lerp(pointA, pointB, Time.deltaTime);

                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator ControlShaftLight()
        {
            while (true)
            {
                yield return new WaitForSeconds(lightLoop);
                shaftLight.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                shaftLight.transform.position = _initLightPosition;
            }
        }
    }
}