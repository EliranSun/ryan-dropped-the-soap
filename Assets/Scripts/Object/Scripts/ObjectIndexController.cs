using TMPro;
using UnityEngine;

namespace Object.Scripts
{
    public class ObjectIndexController : MonoBehaviour
    {
        [SerializeField] private TextMeshPro floorNumberText;

        public void UpdateFloorNumber(int floorNumber)
        {
            floorNumberText.text = floorNumber < 10 ? "0" + floorNumber : floorNumber.ToString();
        }

        public string GetFloorNumberText()
        {
            return floorNumberText.text;
        }
    }
}