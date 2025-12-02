using System;
using TMPro;
using UnityEngine;

namespace Player
{
    [Serializable]
    public class Interaction
    {
        public int objectId;
        public ObjectNames objectName;
    }

    public class InteractionsTrigger : ObserverSubject
    {
        [SerializeField] private TextMeshPro interactionText;
        private ObjectNames _interactedObjectName;
        private int _interactedObjectNameId;

        private void Start()
        {
            if (interactionText) interactionText.text = "";
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X) && _interactedObjectName != ObjectNames.None)
            {
                print($"Notifying player interaction with {_interactedObjectName}");
                var interaction = new Interaction
                {
                    objectId = _interactedObjectNameId,
                    objectName = _interactedObjectName
                };

                Notify(GameEvents.PlayerInteraction, interaction);
            }
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

            InstructBasedOnTag(other.gameObject);
        }

        private void ResetText()
        {
            interactionText.text = "";
            _interactedObjectName = ObjectNames.None;
        }

        private void InstructBasedOnTag(GameObject interactedGameObject)
        {
            var tagName = interactedGameObject.tag;
            switch (tagName)
            {
                case "Building Entrance Doors":
                    interactionText.text = "OPEN";
                    _interactedObjectName = ObjectNames.BuildingEntranceDoors;
                    break;

                case "Building Entrance":
                    interactionText.text = "Press X to ENTER";
                    _interactedObjectName = ObjectNames.BuildingEntrance;
                    break;

                case "Building Exit":
                    interactionText.text = "EXIT";
                    _interactedObjectName = ObjectNames.BuildingExit;
                    break;

                // case "Apartment Door":
                //     // var doorController = interactedGameObject.GetComponent<>();
                //     interactionText.text = "KNOCK";
                //     _interactedObjectNameId = interactedGameObject.GetInstanceID();
                //     _interactedObjectName = ObjectNames.ApartmentDoor;
                //     break;

                case "Apartment Entrance":
                    interactionText.text = "ENTER";
                    _interactedObjectName = ObjectNames.ApartmentEntrance;
                    break;

                case "Apartment Exit":
                    interactionText.text = "Exit";
                    _interactedObjectName = ObjectNames.ApartmentExit;
                    break;

                case "Elevator Doors":
                    interactionText.text = "CALL";
                    break;

                case "Elevator Entrance":
                    interactionText.text = "ENTER";
                    _interactedObjectName = ObjectNames.ElevatorEnterDoors;
                    break;

                case "Elevator Exit":
                    interactionText.text = "EXIT";
                    _interactedObjectName = ObjectNames.ElevatorExitDoors;
                    break;

                case "Staircase Entrance":
                    interactionText.text = "CLIMB";
                    _interactedObjectName = ObjectNames.StaircaseEntrance;
                    break;

                case "Staircase Exit":
                    interactionText.text = "EXIT";
                    _interactedObjectName = ObjectNames.StaircaseExit;
                    break;

                case "Item":
                    interactionText.text = "OBSERVE";
                    break;

                case "NPC":
                    interactionText.text = "TALK";
                    _interactedObjectName = ObjectNames.Npc;
                    _interactedObjectNameId = interactedGameObject.GetInstanceID();
                    break;
            }
        }
    }
}