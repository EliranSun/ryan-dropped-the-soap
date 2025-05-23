using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Elevator.scripts
{
    public class FloorController : ObserverSubject
    {
        [SerializeField] private TextMeshPro floorNumberText;

        [SerializeField] public Apartment[] apartments;

        // TODO: We might want to unify doors & apartments, via an apartment controller or something like that
        [SerializeField] public DoorController[] doors;
        public int floorNumber;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
                Notify(GameEvents.FloorChange, floorNumber);
        }

        public void SetFloorNumber(int newFloorNumber)
        {
            floorNumber = newFloorNumber;
            if (floorNumberText != null)
                floorNumberText.text = floorNumber.ToString();

            for (var i = 0; i <= apartments.Length - 1; i++)
            {
                apartments[i].apartmentNumber = i;
                apartments[i].floorNumber = floorNumber;
                doors[i].SetDoorNumber($"{floorNumber}{i}");
            }
        }

        public void SetObserver(UnityEvent<GameEventData> observer)
        {
            observers = observer;
        }
    }
}