using System;
using Dialog.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dialog.Scripts
{
}

namespace Character_Creator.scripts
{
    [Serializable]
    public enum InteractionType
    {
        Click,
        Move,
        Both
    }

    public class InteractionData
    {
        public readonly NarrationDialogLine DialogLine;
        public readonly InteractableObjectName InteractableObjectName;
        public readonly InteractableObjectType InteractableObjectType;
        public readonly string Name;

        public InteractionData(
            string gameObjectName,
            InteractableObjectName interactableObjectName,
            InteractableObjectType type,
            NarrationDialogLine dialogLine)
        {
            Name = gameObjectName;
            DialogLine = dialogLine;
            InteractableObjectName = interactableObjectName;
            InteractableObjectType = type;
        }
    }

    [RequireComponent(typeof(Collider2D))]
    public class OnInteractableObject : ObserverSubject
    {
        [FormerlySerializedAs("objectName")] [SerializeField]
        private InteractableObjectName interactableObjectName;

        [FormerlySerializedAs("objectType")] [SerializeField]
        private InteractableObjectType interactableObjectType;

        [SerializeField] private GameEvents gameEvent;
        [SerializeField] private NarrationDialogLine[] dialogLine;
        [SerializeField] private InteractionType interactionType;
        [SerializeField] private bool repeatLastInteraction = true;
        [SerializeField] private bool repeatAllInteractions;
        private int _interactionCount;

        private void OnMouseDown()
        {
            if (interactionType is not (InteractionType.Click or InteractionType.Both))
                return;

            print($"CLICK ON {gameObject.name}");

            Notify(gameEvent, new InteractionData(
                gameObject.name,
                interactableObjectName,
                interactableObjectType,
                dialogLine.Length == 0 ? null : dialogLine[_interactionCount]
            ));

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

        private void OnTriggerEnter(Collider other)
        {
            if (interactionType == InteractionType.Move) _interactionCount++;
        }

        public void OnNotify(GameEventData gameEventData)
        {
            switch (gameEventData.Name)
            {
                case GameEvents.ArmchairChosen:
                case GameEvents.DoorChosen:
                case GameEvents.MirrorChosen:
                case GameEvents.PaintingChosen:
                case GameEvents.VaseChosen:
                    var interactionData = new InteractionData(
                        gameObject.name,
                        interactableObjectName,
                        interactableObjectType,
                        dialogLine[_interactionCount]
                    );
                    Notify(gameEventData.Name, interactionData);
                    break;
            }
        }
    }
}