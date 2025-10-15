using TMPro;
using UnityEngine;

namespace Object.Scripts
{
    public class FloorIndexController : MonoBehaviour
    {
        [SerializeField] private TextMeshPro floorNumberText;
        [SerializeField] private GameObject[] doors;
        public int ObjectNumber { get; private set; }

        public void UpdateFloorNumber(int objectNumber)
        {
            ObjectNumber = objectNumber;
             

            if (floorNumberText)
                floorNumberText.text = objectNumber < 10
                    ? "0" + objectNumber
                    : objectNumber.ToString();
        }

        public string GetFloorNumberText()
        {
            return ObjectNumber.ToString();
        }
    }
}