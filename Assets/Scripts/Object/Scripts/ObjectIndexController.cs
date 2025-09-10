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
            floorNumberText.text = objectNumber < 10 ? "0" + objectNumber : objectNumber.ToString();
            ObjectNumber = objectNumber;
        }

        public string GetFloorNumberText()
        {
            return floorNumberText.text;
        }
    }
}