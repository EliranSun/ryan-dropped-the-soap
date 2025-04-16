using System;
using Dialog.Scripts;
using UnityEngine;

namespace common.scripts
{
    [Serializable]
    public enum InteractionType
    {
        ShowHide,
        ClearThoughts,
        Speak
    }


    [RequireComponent(typeof(SpriteRenderer))]
    public class SimpleInteraction : ObserverSubject
    {
        [SerializeField] private InteractableObjectType objectType;
        [SerializeField] private InteractionType interactionType;
        [SerializeField] private bool _disableOnClick = false;
        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnMouseDown()
        {
            // FIXME: This does not work for the flashlight for some reason
            print("Mouse down on" + gameObject.name);
            Notify(GameEvents.ClickOnItem, gameObject.name);
        }

        private void OnMouseUp()
        {
            if (interactionType == InteractionType.ClearThoughts) Notify(GameEvents.ClearThoughts);
            if (interactionType == InteractionType.Speak) Notify(GameEvents.Speak);

            if (_disableOnClick) gameObject.SetActive(false);
        }

        public void OnNotify(GameEventData gameEventData)
        {
            if (gameEventData.Name != GameEvents.ObjectClicked) return;
            if ((InteractableObjectType)gameEventData.Data != objectType) return;

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