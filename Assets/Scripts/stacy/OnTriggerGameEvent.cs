using Dialog;
using UnityEngine;

namespace stacy
{
    public class OnTriggerGameEvent : ObserverSubject
    {
        [SerializeField] private string tagToDetect;
        [SerializeField] private GameEvents gameEventName;
        [SerializeField] private int delayEventTrigger;
        [SerializeField] private bool invokeOnce = true;
        [SerializeField] private NarrationDialogLine toggleTriggerLine;
        private bool _isInvoked;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (invokeOnce && _isInvoked) return;
            if (!collision.gameObject.CompareTag(tagToDetect)) return;

            _isInvoked = true;
            Invoke(nameof(NotifyEvent), delayEventTrigger);
        }

        private void NotifyEvent()
        {
            if (toggleTriggerLine) toggleTriggerLine.lineCondition.isMet = true;

            Notify(gameEventName);
        }
    }
}