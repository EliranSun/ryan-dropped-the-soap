using System;
using UnityEngine;

namespace interactions
{
    [Serializable]
    public class InteractionContext
    {
        // public MiniGameName miniGameName;
        public bool isMiniGameActive;
    }

    public class InteractionSystem : MonoBehaviour
    {
        [SerializeField] public InteractionContext interactionContext;

        public void Request(ObjectInteractionType objectInteractionType)
        {
            print("InteractionSystem request: " + objectInteractionType);
        }
    }
}