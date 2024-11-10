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


    [RequireComponent(typeof(SpriteRenderer))]
    public class SimpleInteraction : MonoBehaviour
    {
        [SerializeField] private InteractableObjectType objectType;
        [SerializeField] private InteractionType interactionType;
        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void OnNotify(GameEventData gameEventData)
        {
            if (gameEventData.name != GameEvents.ObjectClicked) return;
            if ((InteractableObjectType)gameEventData.data != objectType) return;

            switch (interactionType)
            {
                case InteractionType.ShowHide:
                    // gameObject.SetActive(!gameObject.activeSelf);
                    _spriteRenderer.enabled = !_spriteRenderer.enabled;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}