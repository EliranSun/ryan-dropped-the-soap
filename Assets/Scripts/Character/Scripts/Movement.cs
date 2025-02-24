using Object.Scripts;
using UnityEngine;

namespace Character.Scripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private GameObject headGameObject;
        [SerializeField] private GameObject hairGameObject;
        [SerializeField] private float jumpForce = 20f;
        [SerializeField] private bool isRigidBodyMovement = true;
        [SerializeField] private BoatController boatController;

        private SpriteRenderer _hairSpriteRenderer;
        private SpriteRenderer _headSpriteRenderer;

        private bool _isOnBoat;

        private Rigidbody2D _rigidbody2D;
        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            if (headGameObject) _headSpriteRenderer = headGameObject.GetComponent<SpriteRenderer>();
            if (hairGameObject) _hairSpriteRenderer = hairGameObject.GetComponent<SpriteRenderer>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (isRigidBodyMovement)
                RigidBodyMovement();
            else
                CharacterControllerMovement();

            HandleHeadAndHair();

            if (_isOnBoat)
            {
                if (isRigidBodyMovement)
                {
                    _rigidbody2D.velocity = new Vector2(boatController.speed, _rigidbody2D.velocity.y);
                }
                else
                {
                    var translate = new Vector3(boatController.speed, 0, 0) * Time.deltaTime;
                    transform.Translate(translate);
                }
                return;
            }
        }

        private void RigidBodyMovement()
        {
            var horizontal = Input.GetAxis("Horizontal");

            if (horizontal != 0)
                _rigidbody2D.velocity = new Vector2(horizontal * speed, _rigidbody2D.velocity.y);

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                _rigidbody2D.AddForce(Vector2.up * (speed * jumpForce), ForceMode2D.Impulse);
        }

        private void CharacterControllerMovement()
        {
            var horizontal = Input.GetAxis("Horizontal");
            transform.Translate(new Vector3(horizontal, 0, 0) * (speed * Time.deltaTime));
        }

        private void HandleHeadAndHair()
        {
            if (!headGameObject && !hairGameObject) return;

            var horizontal = Input.GetAxis("Horizontal");
            var oldHeadPosition = headGameObject ? headGameObject.transform.localPosition : Vector3.zero;
            var oldHairPosition = hairGameObject ? hairGameObject.transform.localPosition : Vector3.zero;

            switch (horizontal)
            {
                case > 0:
                    _spriteRenderer.flipX = false;
                    if (headGameObject) oldHeadPosition.x = 0.3f;
                    if (hairGameObject) oldHairPosition.x = 0.3f;
                    break;
                case < 0:
                    _spriteRenderer.flipX = true;
                    if (headGameObject) oldHeadPosition.x = -0.3f;
                    if (hairGameObject) oldHairPosition.x = -0.3f;
                    break;
                case 0:
                    if (headGameObject) oldHeadPosition.x = 0;
                    if (hairGameObject) oldHairPosition.x = 0;
                    break;
            }

            if (headGameObject) headGameObject.transform.localPosition = oldHeadPosition;
            if (hairGameObject) hairGameObject.transform.localPosition = oldHairPosition;
            if (headGameObject) _headSpriteRenderer.flipX = _spriteRenderer.flipX;
            if (hairGameObject) _hairSpriteRenderer.flipX = _spriteRenderer.flipX;
        }

        public void OnNotify(GameEventData gameEvent)
        {
            if (gameEvent.name == GameEvents.BoatStart)
                _isOnBoat = true;
        }
    }
}