using System;
using Dialog;
using TMPro;
using UnityEngine;

namespace interactions
{
    [Serializable]
    public enum PlayerInteractionType
    {
        Click,
        Move,
        Keyboard,
        Both
    }

    public enum ObjectInteractionType
    {
        None,
        Flirt,
        Avoid,
        Neglect,
        Attend,
        Shout,
        Talk
    }


    [Serializable]
    public class InteractionOption
    {
        public bool toggle;
        public ObjectInteractionType interaction;
        public InteractionCondition condition;

        public bool IsValid(InteractionContext context)
        {
            return condition.Evaluate(context, toggle);
        }
    }


    [RequireComponent(typeof(Collider2D))]
    public class InteractableObject : ObserverSubject
    {
        [SerializeField] private TextMeshPro interactionTextMesh;
        [SerializeField] private PlayerInteractionType playerInteractionType;
        [SerializeField] private InteractionOption[] npcInteractionOptions;
        [SerializeField] private NarrationDialogLine[] dialogLine;
        [SerializeField] private bool repeatLastInteraction = true;
        [SerializeField] private bool repeatAllInteractions;
        private int _interactionCount;

        // [SerializeField] private InteractableObjectName interactableObjectName;
        // [SerializeField] private InteractableObjectType interactableObjectType;
        // [SerializeField] private GameEvents gameEvent;
        private string _interactionName;
        private bool _isTriggered;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X) && playerInteractionType == PlayerInteractionType.Keyboard && _isTriggered)
            {
                print($"PRESS ON {gameObject.name}");
                TriggerInteraction();
            }
        }

        private void OnMouseDown()
        {
            if (playerInteractionType is not (PlayerInteractionType.Click or PlayerInteractionType.Both))
                return;

            print($"CLICK ON {gameObject.name}");
            TriggerInteraction();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (playerInteractionType == PlayerInteractionType.Move)
                // TODO: move the object? unclear
                _interactionCount++;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _isTriggered = true;
            // print($"OnTriggerEnter2D {gameObject.name}");
            // if (interactionTextMesh)
            //     interactionTextMesh.text = string.IsNullOrEmpty(_interactionName) ? "TALK" : _interactionName;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _isTriggered = false;
            print($"OnTriggerExit2D {gameObject.name}");
            if (interactionTextMesh)
                interactionTextMesh.text = "";
        }


        public ObjectInteractionType GetInteraction(InteractionContext context)
        {
            foreach (var option in npcInteractionOptions)
                if (option.IsValid(context))
                    return option.interaction;

            return ObjectInteractionType.None;
        }

        private void TriggerInteraction()
        {
            // Notify(gameEvent, new InteractionData(
            //     gameObject.name,
            //     interactableObjectName,
            //     interactableObjectType,
            //     dialogLine.Length == 0 ? null : dialogLine[_interactionCount]
            // ));

            if (dialogLine.Length == 0)
                // Notify(gameEvent, interactableObjectType);
                return;

            // TODO: _interactionCount & InteractionData might be redundant after the bubble event action change
            if (_interactionCount >= dialogLine.Length)
            {
                if (repeatLastInteraction) _interactionCount = dialogLine.Length - 1;
                else if (repeatAllInteractions) _interactionCount = 0;
                else return;
            }

            _interactionCount++;
        }

        public void OnNotify(GameEventData gameEventData)
        {
            if (gameEventData.Name == GameEvents.PlayerInteractionRequest)
            {
                var requestType = (ObjectInteractionType)gameEventData.Data;
                if (requestType == ObjectInteractionType.Talk)
                    Notify(GameEvents.TriggerSpecificDialogLine, dialogLine[0]);
            }

            // switch (gameEventData.Name)
            // {
            //     case GameEvents.ArmchairChosen:
            //     case GameEvents.DoorChosen:
            //     case GameEvents.MirrorChosen:
            //     case GameEvents.PaintingChosen:
            //     case GameEvents.VaseChosen:
            //         var interactionData = new InteractionData(
            //             gameObject.name,
            //             interactableObjectName,
            //             interactableObjectType,
            //             dialogLine[_interactionCount]
            //         );
            //         Notify(gameEventData.Name, interactionData);
            //         break;
            // }
        }
    }
}