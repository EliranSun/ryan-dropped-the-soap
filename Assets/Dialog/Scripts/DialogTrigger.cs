using UnityEngine;

namespace Dialog.Scripts
{
    [RequireComponent(typeof(Collider2D))]
    public class DialogTrigger : ObserverSubject
    {
        [SerializeField] private GameObject triggeredBy;
        [SerializeField] private GameEvents eventName;
        private bool _isTriggered;


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isTriggered)
                return;

            if (other.gameObject.CompareTag(triggeredBy.gameObject.tag))
            {
                _isTriggered = true;
                Notify(eventName);
            }
        }
    }
}