using TMPro;
using UnityEngine;

namespace Elevator.scripts
{
    // TODO: Should replace FloorController
    public class FloorWrapController : MonoBehaviour
    {
        [SerializeField] private TextMeshPro floorNumberText;
        [SerializeField] private GameObject stairs;
        [SerializeField] private GameObject[] doors;
        [SerializeField] private GameObject[] apartments;
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
                apartments[i].GetComponent<ApartmentController>().SetData(floorNumber, doorNumber);
            }
        }

        public string GetFloorNumberText()
        {
            return ObjectNumber.ToString();
        }
    }
}