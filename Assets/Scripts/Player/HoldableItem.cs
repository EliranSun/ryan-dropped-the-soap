using UnityEngine;

namespace Player
{
    [RequireComponent(
        typeof(Collider2D),
        typeof(Rigidbody2D),
        typeof(SpriteRenderer)
    )]
    public class HoldableItem : ObserverSubject
    {
        [SerializeField] private GameEvents holdGameEvent;
        [SerializeField] private GameEvents releaseGameEvent;
        private bool _isHeld;
        private Transform _playerTransform;

        // TODOS because current implementation make items DontDestroyOnLoad as well,
        // I guess because they inherit it from player once transformed under him
        // 1. TODO: set collider & rigid here ✅
        // 2. holding & releasing just means the item follows/unfollow player location ✅
        // 3. The notify just change the STATE in ItemsManager - for scene reload and location change ✅
        // 4. upon reload/location change, simply load the item in scene/player based on this updated state
        // 5. upon destroy (game exit?) should save state in PlayerPrefs to reload correct data
        // 5a. reload already handled because player DontDestroyOnLoad

        private void Start()
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Update()
        {
            if (_isHeld && _playerTransform)
                transform.position = (Vector2)_playerTransform.position + new Vector2(0, 2);
        }

        private void OnMouseDown()
        {
            if (_isHeld) Release();
            else Hold();
        }

        public void Hold()
        {
            _isHeld = true;
            GetComponent<SpriteRenderer>().sortingOrder = 20;
            GetComponent<Collider2D>().isTrigger = true;
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            Notify(holdGameEvent, gameObject.name);
        }

        private void Release()
        {
            _isHeld = false;
            GetComponent<SpriteRenderer>().sortingOrder = 4;
            Notify(releaseGameEvent, gameObject.name);
        }
    }
}