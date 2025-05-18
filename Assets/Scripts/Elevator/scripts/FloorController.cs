using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Elevator.scripts
{
    public class FloorController : ObserverSubject
    {
        [SerializeField] private TextMeshPro floorNumberText;
        public int floorNumber;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
                Notify(GameEvents.FloorChange, floorNumber);
        }

        public void SetFloorNumber(int newFloorNumber)
        {
            floorNumber = newFloorNumber;
            if (floorNumberText != null) floorNumberText.text = floorNumber.ToString();
        }

        public void SetObserver(UnityEvent<GameEventData> observer)
        {
            observers = observer;
        }
    }
}