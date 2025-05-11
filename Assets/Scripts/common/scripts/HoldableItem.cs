using UnityEngine;

namespace common.scripts
{
    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
    public class HoldableItem : ObserverSubject
    {
        [SerializeField] private GameEvents triggerGameEvent;
        [SerializeField] private Transform playerTransform;
        private Collider2D _collider2D;
        private Rigidbody2D _rigidbody2D;

        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _collider2D = GetComponent<Collider2D>();
        }

        private void OnMouseDown()
        {
            if (transform.parent == playerTransform)
            {
                _rigidbody2D.isKinematic = false;
                _collider2D.isTrigger = false;
                transform.SetParent(null);
                return;
            }

            _rigidbody2D.isKinematic = true;
            _collider2D.isTrigger = true;
            transform.SetParent(playerTransform);
            transform.localPosition = new Vector2(1f, 1.5f);
            Notify(triggerGameEvent);
        }
    }
}