using TMPro;
using UnityEngine;

namespace Elevator.scripts
{
    public class FloorIndexController : MonoBehaviour
    {
        [SerializeField] private TextMeshPro floorNumberText;
        [SerializeField] private GameObject[] doors;
        public int ObjectNumber { get; private set; }

        public void UpdateFloorNumber(int floorNumber)
        {
            ObjectNumber = floorNumber;


            if (floorNumberText)
                floorNumberText.text = floorNumber < 10
                    ? "0" + floorNumber
                    : floorNumber.ToString();

            for (var i = 0; i < doors.Length; i++)
            {
                var doorNumber = floorNumber * 100 + i + 1;
                doors[i].GetComponent<DoorController>().SetDoorNumber(doorNumber.ToString());
            }
        }

        public string GetFloorNumberText()
        {
            return ObjectNumber.ToString();
        }
    }
}