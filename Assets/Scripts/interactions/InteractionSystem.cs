using System;
using UnityEngine;

namespace interactions
{
    [Serializable]
    public class InteractionContext
    {
        public bool isMiniGameActive;
        public bool isGoodEmployeeFlow;
    }

    public class InteractionSystem : ObserverSubject
    {
        [SerializeField] public InteractionContext interactionContext;

        public void Request(ObjectInteractionType objectInteractionType)
        {
            Notify(GameEvents.PlayerInteractionRequest, objectInteractionType);
        }
    }
}