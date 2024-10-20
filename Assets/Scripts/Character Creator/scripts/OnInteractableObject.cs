using System;
using Dialog.Scripts;
using UnityEngine;

namespace Character_Creator.scripts
{
    [Serializable]
    public enum InteractionType
    {
        Click,
        Move,
        Both,
    }

    public class InteractionData
    {
        public string Name;
        public NarrationDialogLine DialogLine;
        
        public InteractionData(string name, NarrationDialogLine dialogLine)
        {
            Name = name;
            DialogLine = dialogLine;
        }
    }
    
    [RequireComponent(typeof(Collider2D))]
    public class OnInteractableObject : ObserverSubject
    {
        [SerializeField] private GameEvents gameEvent;
        [SerializeField] private NarrationDialogLine[] dialogLine;
        [SerializeField] private InteractionType interactionType;
        private int _interactionCount;
        [SerializeField] private bool repeatLastInteraction = true;

        private void OnMouseDown()
        {
            if (interactionType is InteractionType.Click or InteractionType.Both)
            {
                if (_interactionCount >= dialogLine.Length)
                {
                    if (repeatLastInteraction)
                    {
                        _interactionCount = dialogLine.Length - 1;
                    }
                    else
                    {
                        return;
                    }
                }
                
                Notify(gameEvent, new InteractionData(gameObject.name, dialogLine[_interactionCount]));
                _interactionCount++;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (interactionType == InteractionType.Move)
            {
                _interactionCount++;
            }
        }
    }
}