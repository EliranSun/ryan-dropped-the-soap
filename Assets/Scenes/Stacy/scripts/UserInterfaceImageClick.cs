using Mini_Games.Organize_Desk.scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UserInterfaceImageClick : ObserverSubject
{
    [SerializeField] private GameEvents eventName;
    [SerializeField] public UIItem uiItemName;

    void Start()
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
        print("CLICK ON UI");
        if (eventName != GameEvents.None)
            Notify(eventName, uiItemName);
    }
}
