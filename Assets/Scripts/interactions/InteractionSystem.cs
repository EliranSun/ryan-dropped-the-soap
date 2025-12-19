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

    public class InteractionSystem : MonoBehaviour
    {
        [SerializeField] public InteractionContext interactionContext;

        public void Request(ObjectInteractionType objectInteractionType)
        {
            print("InteractionSystem request: " + objectInteractionType);
            if (interactionContext.isMiniGameActive && interactionContext.isGoodEmployeeFlow)
            {
                // trigger mini game
            }
        }
    }
}