using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
    public class Item : ObserverSubject
    {
        // Static dictionary to track all item instances by their name
        private static readonly Dictionary<string, Item> ItemInstances = new();

        [SerializeField] private GameEvents holdGameEvent;
        [SerializeField] private GameEvents releaseGameEvent;

        // [SerializeField] private PlayerStatesController playerStatesController;

        // private bool _isHeld;
        private Collider2D _collider2D;
        private int _itemRoomNumber;
        private Transform _playerTransform;
        private Rigidbody2D _rigidbody2D;

        private void Awake()
        {
            // Check if an instance with this name already exists
            if (ItemInstances.TryGetValue(gameObject.name, out var existingItem))
            {
                // If this item already exists, destroy this new instance
                Destroy(gameObject);
                return;
            }

            // If this is the first instance, add it to our dictionary
            ItemInstances[gameObject.name] = this;
        }

        private void Start()
        {
            _playerTransform = PlayerStatesController.Instance.transform;
            // PlayerPrefs.SetInt("Wanderer-room", 420);
            //
            // _itemRoomNumber = PlayerPrefs.GetInt($"{gameObject.name}-room");
            _collider2D = GetComponent<Collider2D>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            // var currentPlayerApartment = playerStatesController.GetCurrentApartmentNumber();
            //

            PositionItem();

            // if (_isHeld) Hold();
            // // else 
            // if (_itemRoomNumber == currentPlayerApartment)
            // {
            //     Release();
            //     
            // }
        }

        private void OnDestroy()
        {
            // Remove this instance from the dictionary when destroyed
            if (ItemInstances.ContainsKey(gameObject.name) && ItemInstances[gameObject.name] == this)
                ItemInstances.Remove(gameObject.name);
        }

        private void OnMouseDown()
        {
            if (transform.parent == _playerTransform)
            {
                Release();
                return;
            }

            Hold();
        }

        private void Hold()
        {
            DontDestroyOnLoad(gameObject);

            // SetHoldingItem();
            // DeleteStoredPosition();
            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            _collider2D.isTrigger = true;
            // We already have player reference through _playerTransform
            transform.SetParent(_playerTransform);
            transform.position = (Vector2)_playerTransform.position + new Vector2(0f, 3f);
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = 20;
            Notify(holdGameEvent, gameObject.transform.position);
        }

        private void Release()
        {
            StorePosition();
            // _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            _collider2D.isTrigger = false;
            transform.SetParent(null);
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = 4;
            Invoke(nameof(NotifyRelease), 1f);
        }

        private void NotifyRelease()
        {
            Notify(releaseGameEvent, gameObject.transform.position);
        }

        // private void SetHoldingItem()
        // {
        //     PlayerPrefs.SetString("PlayerHoldingItem", gameObject.name);
        // }

        private void StorePosition()
        {
            PlayerPrefs.SetString($"{gameObject.name}-position",
                $"{gameObject.transform.position.x},{gameObject.transform.position.y}");
        }

        private Vector2 GetStoredPosition()
        {
            var storedPosition = PlayerPrefs.GetString($"{gameObject.name}-position");

            if (storedPosition == "")
                return Vector2.zero;

            var positionParts = storedPosition.Split(',');

            if (
                positionParts.Length == 2 &&
                float.TryParse(positionParts[0], out var x) &&
                float.TryParse(positionParts[1], out var y)
            )
                return new Vector2(x, y);

            return Vector2.zero;
        }

        private void PositionItem()
        {
            var position = GetStoredPosition();

            if (position != Vector2.zero)
            {
                _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
                _collider2D.isTrigger = true;
                gameObject.transform.position = position;
            }
        }
    }
}