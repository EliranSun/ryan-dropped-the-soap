using TMPro;
using UnityEngine;

namespace Elevator.scripts
{
    public class FloorController : MonoBehaviour
    {
        [SerializeField] private TextMeshPro floorNumberText;
        private int _floorNumber;

        public void SetFloorNumber(int floorNumber)
        {
            _floorNumber = floorNumber;
            if (floorNumberText != null) floorNumberText.text = _floorNumber.ToString();
        }
    }
}