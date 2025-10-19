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

        private void OnTriggerExit2D(Collider2D other)
        {
            ResetText();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player") ||
                other.CompareTag("Ground") ||
                other.CompareTag("Untagged"))
                return;

            InstructBasedOnTag(other.gameObject.tag);
        }

        private void ResetText()
        {
            interactionText.text = "";
            _interactedObjectName = ObjectNames.None;
        }

        private void InstructBasedOnTag(string tagName)
        {
            switch (tagName)
            {
                case "Building Entrance":
                    interactionText.text = "Press X to ENTER";
                    _interactedObjectName = ObjectNames.BuildingEntrance;
                    break;

                case "Building Exit":
                    interactionText.text = "EXIT";
                    _interactedObjectName = ObjectNames.BuildingExit;
                    break;

                case "Building Entrance Doors":
                    interactionText.text = "OPEN";
                    _interactedObjectName = ObjectNames.BuildingEntranceDoors;
                    break;

                case "Door":
                    interactionText.text = "KNOCK";
                    break;

                case "Opened Door":
                    interactionText.text = "ENTER";
                    break;

                case "Item":
                    interactionText.text = "OBSERVE";
                    break;

                case "Elevator Exit":
                    interactionText.text = "EXIT";
                    _interactedObjectName = ObjectNames.ElevatorExitDoors;
                    break;

                case "Elevator Entrance":
                    interactionText.text = "ENTER";
                    _interactedObjectName = ObjectNames.ElevatorEnterDoors;
                    break;

                case "Staircase Entrance":
                    interactionText.text = "CLIMB";
                    _interactedObjectName = ObjectNames.StaircaseEntrance;
                    break;

                case "Staircase Exit":
                    interactionText.text = "EXIT";
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