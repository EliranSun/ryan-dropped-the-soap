using UnityEngine;

namespace common.scripts
{
    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
    public class HoldableItem : ObserverSubject
    {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private GameEvents holdGameEvent;
        [SerializeField] private GameEvents releaseGameEvent;
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
                _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                _collider2D.isTrigger = false;
                transform.SetParent(null);
                Notify(releaseGameEvent);
                return;
            }

            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            _collider2D.isTrigger = true;
            transform.SetParent(playerTransform);
            transform.localPosition = new Vector2(1f, 1.5f);
            Notify(holdGameEvent);
        }
    }
}