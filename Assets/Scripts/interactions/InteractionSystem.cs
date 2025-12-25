using System;
using UnityEngine;

namespace interactions
{
    [Serializable]
    public class InteractionContext
    {
        public string miniGameName;
        public bool isMiniGameActive;
        public bool isGoodEmployeeFlow;
    }

    public class InteractionSystem : ObserverSubject
    {
        [SerializeField] public InteractionContext interactionContext;

        public void Request(ActorInteraction actorInteraction)
        {
            Notify(GameEvents.PlayerInteractionRequest, actorInteraction);
        }
    }
}