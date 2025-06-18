using System;
using Dialog;
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

        [FormerlySerializedAs("_disableOnClick")]
        [SerializeField]
        private bool disableGameObjectOnClick;

        [SerializeField] public bool isEnabled = true;
        [SerializeField] private LayerMask interactableLayerMask = -1; // Which layers can be interacted with
        [SerializeField] private bool _is3D = false; // Set to true if this is a 3D object

        private SpriteRenderer _spriteRenderer;
        private Camera _mainCamera;
        private Collider _collider3D;
        private Collider2D _collider2D;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _mainCamera = Camera.main;

            if (_is3D)
            {
                _collider3D = GetComponent<Collider>();
                if (_collider3D == null)
                {
                    Debug.LogError("SimpleInteraction: 3D mode requires a Collider component");
                    enabled = false;
                }
            }
            else
            {
                _collider2D = GetComponent<Collider2D>();
                if (_collider2D == null)
                {
                    Debug.LogError("SimpleInteraction: 2D mode requires a Collider2D component");
                    enabled = false;
                }
            }
        }

        private void Update()
        {
            HandleMouseInput();
        }

        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (IsMouseOverThisObject())
                {
                    HandleMouseDown();
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (IsMouseOverThisObject())
                {
                    HandleMouseUp();
                }
            }
        }

        private bool IsMouseOverThisObject()
        {
            if (!isEnabled) return false;

            if (_is3D)
            {
                Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, interactableLayerMask);

                // Check if this object is the closest interactable object
                float closestDistance = Mathf.Infinity;
                GameObject closestInteractable = null;

                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.GetComponent<SimpleInteraction>() != null && hit.distance < closestDistance)
                    {
                        closestDistance = hit.distance;
                        closestInteractable = hit.collider.gameObject;
                    }
                }

                return closestInteractable == gameObject;
            }
            else
            {
                Vector2 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D[] hits = Physics2D.RaycastAll(mouseWorldPos, Vector2.zero, 0f, interactableLayerMask);

                // Check if this object is the topmost interactable object (closest to camera)
                float closestZ = Mathf.Infinity;
                GameObject closestInteractable = null;

                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider.GetComponent<SimpleInteraction>() != null)
                    {
                        float zPos = hit.collider.transform.position.z;
                        if (zPos < closestZ) // In 2D, smaller Z values are closer to camera
                        {
                            closestZ = zPos;
                            closestInteractable = hit.collider.gameObject;
                        }
                    }
                }

                return closestInteractable == gameObject;
            }
        }

        private void HandleMouseDown()
        {
            if (!isEnabled) return;
            print("Mouse down on " + gameObject.name);
            Notify(GameEvents.ClickOnItem, gameObject.name);
        }

        private void HandleMouseUp()
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