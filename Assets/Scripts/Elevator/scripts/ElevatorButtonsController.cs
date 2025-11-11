using TMPro;
using UnityEngine;

namespace Elevator.scripts
{
    public class ElevatorButtonsController : MonoBehaviour
    {
        [SerializeField] private ElevatorController elevatorController;
        [SerializeField] public TextMeshPro floorText;
        [SerializeField] private TextMeshPro desiredFloorText;

        private void Start()
        {
            if (floorText)
                floorText.text = elevatorController.currentFloor.ToString();
        }

        private void UpdateFloor(int floorNumber)
        {
            if (elevatorController.isFloorMoving)
                return;

            desiredFloorText.text = desiredFloorText.text == "00" // init state
                ? $"{floorNumber}"
                : $"{desiredFloorText.text}{floorNumber}";


            elevatorController.targetFloor = int.Parse(desiredFloorText.text);
        }

        public void OnElevatorButtonClick(string buttonNumberString)
        {
            if (buttonNumberString == "go")
            {
                elevatorController.GoToFloor(elevatorController.targetFloor);
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