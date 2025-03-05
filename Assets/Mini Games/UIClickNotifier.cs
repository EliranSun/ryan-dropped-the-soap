using Mini_Games.Organize_Desk.scripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Mini_Games
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(RectTransform))]
    public class UIClickNotifier : ObserverSubject, IPointerClickHandler
    {
        [SerializeField] private GameEvents gameEventName;
        [SerializeField] public UIItem uiItemName;
        private bool _isSelected;

        private void Start()
        {
            // Check if we're in a Canvas hierarchy
            var canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("MiniGameTrigger must be placed inside a Canvas hierarchy for UI interaction to work!");
                return;
            }

            // Ensure the Canvas has a GraphicRaycaster
            if (canvas.GetComponent<GraphicRaycaster>() == null)
            {
                Debug.LogWarning("Canvas needs a GraphicRaycaster for UI interaction to work! Adding one...");
                canvas.gameObject.AddComponent<GraphicRaycaster>();
            }

            // Make sure our Image component has raycast target enabled
            var image = GetComponent<Image>();
            if (image != null && !image.raycastTarget)
            {
                Debug.LogWarning("Image raycastTarget is disabled! Enabling it for click detection...");
                image.raycastTarget = true;
            }

            // Make sure there's an EventSystem in the scene
            if (FindObjectOfType<EventSystem>() == null)
            {
                Debug.LogWarning("No EventSystem found in the scene! UI interactions require an EventSystem.");
                var eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_isSelected)
                RemoveDuplicateItem();
            else
                DuplicateItem();

            _isSelected = !_isSelected;
            Notify(gameEventName, gameObject);
        }

        private void DuplicateItem()
        {
            var item = Instantiate(gameObject, transform.parent);
            GetComponent<Image>().color = Color.black;

            // disable the item's raycast target
            item.GetComponent<Image>().raycastTarget = false;
            item.transform.position = transform.position + new Vector3(4f, 4f, 0f);
            item.transform.SetParent(transform);
        }

        private void RemoveDuplicateItem()
        {
            Destroy(transform.GetChild(0).gameObject);
            GetComponent<Image>().color = Color.white;
        }

        // New overload that accepts an Action<GameEventData> delegate
        public void SetUIItemData(UIItem itemName, GameEvents clickEventName, UnityAction<GameEventData> notifyCallback)
        {
            gameEventName = clickEventName;
            uiItemName = itemName;

            if (notifyCallback != null)
                // Register the callback directly to receive notifications
                observers.AddListener(notifyCallback);
        }
    }
}