using System;
using Dialog.Scripts;
using UnityEngine;

namespace common.scripts
{
    [Serializable]
    public enum InteractionType
    {
        ShowHide
    }


    public class SimpleInteraction : MonoBehaviour
    {
        [SerializeField] private InteractableObjectType objectType;
        [SerializeField] private InteractionType interactionType;

        public void OnNotify(GameEventData gameEventData)
        {
            if (gameEventData.name != GameEvents.ObjectClicked) return;
            if ((InteractableObjectType)gameEventData.data != objectType) return;

            switch (interactionType)
            {
                case InteractionType.ShowHide:
                    gameObject.SetActive(!gameObject.activeSelf);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}