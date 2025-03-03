using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Mini_Games
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(RectTransform))]
    public class MiniGameTrigger : ObserverSubject, IPointerClickHandler
    {
        [SerializeField] private GameEvents gameEventName;

        private void Start()
        {
            // Check if we're in a Canvas hierarchy
            Canvas canvas = GetComponentInParent<Canvas>();
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
            Image image = GetComponent<Image>();
            if (image != null && !image.raycastTarget)
            {
                Debug.LogWarning("Image raycastTarget is disabled! Enabling it for click detection...");
                image.raycastTarget = true;
            }

            // Make sure there's an EventSystem in the scene
            if (FindObjectOfType<EventSystem>() == null)
            {
                Debug.LogWarning("No EventSystem found in the scene! UI interactions require an EventSystem.");
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Notify(gameEventName);
        }
    }
}