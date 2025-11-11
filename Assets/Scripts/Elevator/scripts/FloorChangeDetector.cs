using UnityEngine;

namespace Elevator.scripts
{
    public class FloorChangeDetector : ObserverSubject
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Elevator Floor Trigger"))
            {
                var floorNumber = other.gameObject.GetComponent<FloorController>().ObjectNumber;
                Notify(GameEvents.FloorChange, floorNumber);
            }
        }
    }
}