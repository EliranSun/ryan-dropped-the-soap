using TMPro;
using UnityEngine;

namespace Elevator.scripts
{
    public class FloorController : MonoBehaviour
    {
        [SerializeField] private TextMeshPro floorNumberText;
        [SerializeField] private TextMeshPro stairsFloorNumberText;
        [SerializeField] private DoorController[] doors;
        [SerializeField] private ApartmentController[] apartments;
        public int ObjectNumber { get; private set; }

        public void UpdateFloorNumber(int floorNumber)
        {
            ObjectNumber = floorNumber;

            var floorNumberLabel = GetFloorNumberLabel(floorNumber);

            floorNumberText.text = floorNumberLabel;
            stairsFloorNumberText.text = floorNumberLabel;

            for (var i = 0; i < doors.Length; i++)
            {
                var doorNumber = floorNumber * 100 + i + 1;
                doors[i].SetDoorNumber(doorNumber.ToString());
                apartments[i].SetData(floorNumber, doorNumber);
            }
        }

        private string GetFloorNumberLabel(int floorNumber)
        {
            return floorNumber < 10
                ? "0" + floorNumber
                : floorNumber.ToString();
        }

        public string GetFloorNumberText()
        {
            return ObjectNumber.ToString();
        }
    }
}