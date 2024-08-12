using UnityEngine;

namespace Character.Scripts
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerWorldInteractions : ObserverSubject
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Elevator Doors"))
                Notify(GameEvents.PlayerExitElevator);
        }
    }
}
