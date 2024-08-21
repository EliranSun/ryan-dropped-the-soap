using UnityEngine;

namespace Character.Scripts
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerWorldInteractions : ObserverSubject
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            switch (other.tag)
            {
                case "Inside Elevator":
                    Notify(GameEvents.PlayerInsideElevator);
                    break;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            switch (other.tag)
            {
                case "Inside Elevator":
                    Notify(GameEvents.PlayerExitElevator);
                    break;
            }
        }
    }
}