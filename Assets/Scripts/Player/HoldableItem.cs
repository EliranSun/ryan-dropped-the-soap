using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
    public class HoldableItem : ObserverSubject
    {
        [SerializeField] private GameEvents holdGameEvent;
        [SerializeField] private GameEvents releaseGameEvent;

        private void OnMouseDown()
        {
            Hold();
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.PlayerReleaseItem)
            {
                var itemName = (string)eventData.Data;
                if (itemName == gameObject.name) Release();
            }
        }

        private void Hold()
        {
            gameObject.SetActive(false);
            Notify(holdGameEvent, gameObject.name);
        }

        private void Release()
        {
            gameObject.SetActive(true);
            Invoke(nameof(NotifyRelease), 1f);
        }

        private void NotifyRelease()
        {
            Notify(releaseGameEvent, gameObject.name);
        }
    }
}