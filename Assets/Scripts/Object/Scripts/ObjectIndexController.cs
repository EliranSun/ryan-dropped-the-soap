using TMPro;
using UnityEngine;

namespace Object.Scripts
{
    public class ObjectIndexController : MonoBehaviour
    {
        [SerializeField] private TextMeshPro floorNumberText;
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