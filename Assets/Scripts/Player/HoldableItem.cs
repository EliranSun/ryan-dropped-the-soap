using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
    public class HoldableItem : ObserverSubject
    {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private GameEvents holdGameEvent;
        [SerializeField] private GameEvents releaseGameEvent;
        [SerializeField] private PlayerStatesController playerStatesController;
        [SerializeField] private bool isHeld;
        private Collider2D _collider2D;
        private int _itemRoomNumber;
        private Rigidbody2D _rigidbody2D;

        private void Start()
        {
            // PlayerPrefs.SetInt("Wanderer-room", 420);
            //
            // _itemRoomNumber = PlayerPrefs.GetInt($"{gameObject.name}-room");
            _collider2D = GetComponent<Collider2D>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            // var currentPlayerApartment = playerStatesController.GetCurrentApartmentNumber();
            //

            if (isHeld) Hold();
            // // else 
            // if (_itemRoomNumber == currentPlayerApartment)
            // {
            //     Release();
            //     PositionItem();
            // }
        }

        private void OnMouseDown()
        {
            if (transform.parent == playerTransform)
            {
                Release();
                return;
            }

            Hold();
        }

        private void Hold()
        {
            SetHoldingItem();
            // DeleteStoredPosition();
            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            _collider2D.isTrigger = true;
            transform.SetParent(playerTransform);
            transform.position = (Vector2)playerTransform.position + new Vector2(0f, 3f);
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

        private void SetHoldingItem()
        {
            PlayerPrefs.SetString("PlayerHoldingItem", gameObject.name);
        }

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
            gameObject.SetActive(true);
            gameObject.transform.position = GetStoredPosition();
        }
    }
}