using System;
using Dialog.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace common.scripts
{
    [Serializable]
    public enum InteractionType
    {
        ShowHide,
        ClearThoughts,
        Speak,
        None
    }


    [RequireComponent(typeof(SpriteRenderer))]
    public class SimpleInteraction : ObserverSubject
    {
        [SerializeField] private InteractableObjectType objectType;
        [SerializeField] private InteractionType interactionType;

        [FormerlySerializedAs("_disableOnClick")] [SerializeField]
        private bool disableGameObjectOnClick;

        [SerializeField] public bool isEnabled = true;
        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnMouseDown()
        {
            if (!isEnabled) return;
            // FIXME: This does not work for the flashlight for some reason
            print("Mouse down on" + gameObject.name);
            Notify(GameEvents.ClickOnItem, gameObject.name);
        }

        private void OnMouseUp()
        {
            if (!isEnabled) return;

            if (interactionType == InteractionType.ClearThoughts) Notify(GameEvents.ClearThoughts);
            if (interactionType == InteractionType.Speak) Notify(GameEvents.Speak);

            if (disableGameObjectOnClick)
                // gameObject.SetActive(false);
                _spriteRenderer.enabled = !_spriteRenderer.enabled;
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