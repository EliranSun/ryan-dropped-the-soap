using Object.Scripts;
using TMPro;
using UnityEngine;

namespace Player
{
    public class InteractionsTrigger : ObserverSubject
    {
        [SerializeField] private TextMeshPro interactionText;
        private ObjectNames _interactedObjectName;

        private void Start()
        {
            interactionText.text = "";
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X) && _interactedObjectName != ObjectNames.None)
                Notify(GameEvents.PlayerInteraction, _interactedObjectName);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            InstructBasedOnTag(other.gameObject);
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            ResetText(other.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            InstructBasedOnTag(other.gameObject);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            ResetText(other.gameObject);
        }

        private void ResetText(GameObject other)
        {
            if (other.CompareTag("Apartment Door") ||
                other.CompareTag("Door") ||
                other.CompareTag("Building Entrance") ||
                other.CompareTag("Elevator") ||
                other.CompareTag("Staircase Entrance") ||
                other.CompareTag("NPC"))
            {
                interactionText.text = "";
                _interactedObjectName = ObjectNames.None;
            }
        }

        private void InstructBasedOnTag(GameObject other)
        {
            if (other.CompareTag("Building Entrance"))
            {
                interactionText.text = "Press X to ENTER";
                _interactedObjectName = ObjectNames.BuildingEntrance;
            }

            if (other.CompareTag("Door"))
            {
                interactionText.text = "OPEN";
                _interactedObjectName = ObjectNames.BuildingEntranceDoors;
            }

            if (other.CompareTag("Item")) interactionText.text = "OBSERVE";
            if (other.CompareTag("Elevator"))
            {
                interactionText.text = "CALL";
                _interactedObjectName = ObjectNames.Elevator;
            }

            if (other.CompareTag("Staircase Entrance")) interactionText.text = "CLIMB";
            if (other.CompareTag("Apartment Door")) interactionText.text = "KNOCK";
            if (other.CompareTag("NPC")) interactionText.text = "TALK";
        }
    }
}