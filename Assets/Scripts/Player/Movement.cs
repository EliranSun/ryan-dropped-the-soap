using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float jumpForce = 20f;
        [SerializeField] private GameObject headGameObject;
        [SerializeField] private GameObject hairGameObject;
        [SerializeField] private bool isRigidBodyMovement = true;
        [SerializeField] private bool addWobblyMovement;
        [SerializeField] public SpriteRenderer spriteRenderer;
        [SerializeField] public bool allowFlight;
        [SerializeField] public bool resetPlayerStoredPosition;
        private bool _flipEnabled = true;

        private SpriteRenderer _hairSpriteRenderer;
        private SpriteRenderer _headSpriteRenderer;
        private bool _isCrawling;
        private bool _isMoving;
        private bool _isOnGround;

        private InputAction _moveAction;
        private Rigidbody2D _rigidbody2D;

        private void Awake()
        {
            if (resetPlayerStoredPosition) PlayerPrefs.SetString("PlayerPosition", "0,0");
        }

        private void Start()
        {
            _moveAction = InputSystem.actions.FindAction("Move");

            if (headGameObject) _headSpriteRenderer = headGameObject.GetComponent<SpriteRenderer>();
            if (hairGameObject) _hairSpriteRenderer = hairGameObject.GetComponent<SpriteRenderer>();

            _rigidbody2D = GetComponent<Rigidbody2D>();
            // spriteRenderer = GetComponent<SpriteRenderer>();

            if (addWobblyMovement) StartCoroutine(WobblyMovement());
        }

        private void Update()
        {
            if (isRigidBodyMovement) RigidBodyMovement();
            else TransformMovement();

            HandleHeadAndHair();

            PlayerPrefs.SetString("PlayerPosition", $"{transform.position.x},{transform.position.y}");
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("NPC"))
                _isOnGround = true;
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("NPC"))
                _isOnGround = false;
        }

        private void RigidBodyMovement()
        {
            var horizontal = Input.GetAxis("Horizontal");


            if (horizontal != 0)
            {
                var xVelocity = horizontal * speed;
                if (_isCrawling) xVelocity /= 2;

                _rigidbody2D.linearVelocity = new Vector2(xVelocity, _rigidbody2D.linearVelocity.y);
                if (_flipEnabled) spriteRenderer.flipX = horizontal < 0;
                if (_isMoving)
                    return;

                _isMoving = true;
                if (addWobblyMovement) StartCoroutine(WobblyMovement());
            }
            else if (_isMoving)
            {
                _rigidbody2D.linearVelocity = new Vector2(0, _rigidbody2D.linearVelocity.y);
                _isMoving = false;
                transform.rotation = Quaternion.identity;
                StopAllCoroutines();
            }


            if (_isOnGround && (Input.GetKeyDown(KeyCode.W) ||
                                Input.GetKeyDown(KeyCode.UpArrow) ||
                                Input.GetButtonDown("Jump")))
            {
                var force = Vector2.up * jumpForce;
                print($"Adding force {force}");
                _rigidbody2D.AddForce(force, ForceMode2D.Impulse);
            }
        }

        private void TransformMovement()
        {
            // var horizontal = Input.GetAxis("Horizontal");
            var moveValue = _moveAction.ReadValue<Vector2>();

            var vertical = allowFlight ? Input.GetAxis("Vertical") : 0;

            transform.Translate(new Vector3(moveValue.x, vertical, 0) * (speed * Time.deltaTime));

            if (Input.GetKeyDown(KeyCode.W) ||
                Input.GetKeyDown(KeyCode.UpArrow) ||
                Input.GetButtonDown("Jump"))
            {
                var force = Vector2.up * jumpForce;
                _rigidbody2D.AddForce(force, ForceMode2D.Impulse);
            }

            if (moveValue.x == 0) return;

            if (_flipEnabled && spriteRenderer)
                spriteRenderer.flipX = moveValue.x < 0;
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
                    spriteRenderer.flipX = false;
                    if (headGameObject) oldHeadPosition.x = 0.3f;
                    if (hairGameObject) oldHairPosition.x = 0.3f;
                    break;
                case < 0:
                    spriteRenderer.flipX = true;
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
            if (headGameObject) _headSpriteRenderer.flipX = spriteRenderer.flipX;
            if (hairGameObject) _hairSpriteRenderer.flipX = spriteRenderer.flipX;
        }

        private IEnumerator WobblyMovement()
        {
            while (true)
                if (_isMoving)
                {
                    var direction = Mathf.Sign(_rigidbody2D.linearVelocity.x);
                    transform.Rotate(new Vector3(0, 0, -5f * direction));
                    yield return new WaitForSeconds(0.2f);
                    transform.Rotate(new Vector3(0, 0, 5f * direction));
                    yield return new WaitForSeconds(0.2f);
                }
                else
                {
                    yield return null;
                }
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.CrawlTrigger)
            {
                _flipEnabled = !_flipEnabled;
                _isCrawling = true;
            }

            if (eventData.Name == GameEvents.IdleTrigger)
            {
                _flipEnabled = true;
                _isCrawling = false;
            }

            if (eventData.Name == GameEvents.PlayerGrew)
            {
                var bodyProperty = eventData.Data.GetType().GetProperty("body");
                if (bodyProperty == null) return;

                var bodyValue = (GameObject)bodyProperty.GetValue(eventData.Data);
                if (bodyValue)
                    spriteRenderer = bodyValue.GetComponent<SpriteRenderer>();
            }
        }

        public void SlowDown()
        {
            speed /= 4;
        }

        public void NormalSpeed()
        {
            speed *= 4;
        }
    }
}