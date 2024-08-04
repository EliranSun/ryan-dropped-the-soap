using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Elevator.scripts
{
    public class ElevatorController : MonoBehaviour
    {
        [SerializeField] private ElevatorShake shakeableCamera;
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshPro floorText;
        [SerializeField] private TextMeshPro currentFloorText;
        [SerializeField] private float debounce = 3f;
        private int _currentFloor;
        private bool _isMoving;
        private float _timeDiff;
        private float _timeSinceLastClick;

        private void Start()
        {
            var panelChildren = panel.transform.childCount;
            for (var i = 0; i < panelChildren; i++)
            {
                var child = panel.transform.GetChild(i);
                var button = child.GetComponent<Button>();
                button.onClick.AddListener(() => OnElevatorButtonClick(button));
            }
        }

        private void Update()
        {
            _timeSinceLastClick += Time.deltaTime;

            if (_isMoving)
                return;

            if (_currentFloor == int.Parse(floorText.text))
                return;

            if (_timeSinceLastClick == 0)
                return;

            print(_timeSinceLastClick - _timeDiff);

            if (_timeSinceLastClick - _timeDiff < debounce)
                return;

            var floor = int.Parse(floorText.text);
            GoToFloor(floor);
        }

        private void OnElevatorButtonClick(Button button)
        {
            if (_isMoving)
                return;

            _timeSinceLastClick = 0;
            _timeDiff = Time.deltaTime;

            var buttonName = button.name;
            var floor = int.Parse(buttonName);
            floorText.text = floorText.text == "0" // init state
                ? $"{floor}"
                : $"{floorText.text}{floor}";
        }

        private void GoToFloor(int floorNumber)
        {
            print($"Going to floor {floorNumber}");
            StartCoroutine(Move(floorNumber));
            shakeableCamera.Shake(Mathf.Abs(floorNumber - _currentFloor));
        }

        private IEnumerator Move(int floorNumber)
        {
            _isMoving = true;

            while (_currentFloor != floorNumber)
            {
                yield return new WaitForSeconds(1);

                if (_currentFloor < floorNumber)
                    _currentFloor++;
                else
                    _currentFloor--;

                currentFloorText.text = $"{_currentFloor}";
            }

            _isMoving = false;
        }
    }
}