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
            switch (other.tag)
            {
                case "Building Entrance":
                    interactionText.text = "Press X to ENTER";
                    _interactedObjectName = ObjectNames.BuildingEntrance;
                    break;

                case "Door":
                    interactionText.text = "OPEN";
                    _interactedObjectName = ObjectNames.BuildingEntranceDoors;
                    break;

                case "Item":
                    interactionText.text = "OBSERVE";
                    break;

                case "Elevator Entrance":
                    interactionText.text = "EXIT ELEVATOR";
                    _interactedObjectName = ObjectNames.Elevator;
                    break;

                case "Staircase Entrance":
                    interactionText.text = "CLIMB";
                    _interactedObjectName = ObjectNames.StaircaseEntrance;
                    break;

                case "Staircase Exit":
                    interactionText.text = "EXIT STAIRCASE";
                    _interactedObjectName = ObjectNames.StaircaseExit;
                    break;

                case "Apartment Door":
                    interactionText.text = "KNOCK";
                    break;

                case "NPC":
                    interactionText.text = "TALK";
                    break;
            }
        }
    }
}