using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Elevator.scripts
{
    public class ElevatorController : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshPro floorText;
        [SerializeField] private TextMeshPro currentFloorText;
        [SerializeField] private float debounce = 3f;
        private int _currentFloor;
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
            _timeDiff += Time.deltaTime;
            _timeSinceLastClick += Time.deltaTime;

            if (floorText.text == "0")
                return;

            if (_currentFloor != int.Parse(floorText.text))
                return;

            if (!(_timeSinceLastClick > debounce))
                return;

            var floor = int.Parse(floorText.text);
            GoToFloor(floor);
            floorText.text = "0";
        }

        private void OnElevatorButtonClick(Button button)
        {
            _timeSinceLastClick = 0;

            var buttonName = button.name;
            var floor = int.Parse(buttonName);
            floorText.text = floorText.text == "0" // init state
                ? $"{floor}"
                : $"{floorText.text}{floor}";
        }

        private void GoToFloor(int floorNumber)
        {
            _timeSinceLastClick = 0;
            print($"Going to floor {floorNumber}");
            currentFloorText.text = $"{floorNumber}";
        }
    }
}