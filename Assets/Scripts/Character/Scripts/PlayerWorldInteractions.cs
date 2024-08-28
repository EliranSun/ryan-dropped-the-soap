using UnityEngine;

namespace Character.Scripts
{
    [RequireComponent(typeof(Collider2D))]
    public class PlayerWorldInteractions : ObserverSubject
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            print($"Player Collision Enter {other.collider.tag} {other.gameObject.name}");
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            print($"Player Collision Exit {other.collider.tag} {other.gameObject.name}");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            print($"Player Ënter {other.tag} {other.gameObject.name}");
            switch (other.tag)
            {
                case "Inside Elevator":
                    Notify(GameEvents.PlayerInsideElevator);
                    break;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            print($"Player Exit {other.tag} {other.gameObject.name}");
            switch (other.tag)
            {
                case "Inside Elevator":
                    var triggerBounds = GetComponent<Collider2D>().bounds;
                    Vector2 exitPoint = other.transform.position;

                    if (Mathf.Abs(exitPoint.x - triggerBounds.min.x) < Mathf.Abs(exitPoint.y - triggerBounds.min.y))
                        if (exitPoint.x > triggerBounds.center.x)
                            Notify(GameEvents.ExitElevatorToFloors);
                        else if (exitPoint.y > triggerBounds.center.y)
                            Notify(GameEvents.ExitElevatorToLobby);
                    break;
            }
        }
    }
}