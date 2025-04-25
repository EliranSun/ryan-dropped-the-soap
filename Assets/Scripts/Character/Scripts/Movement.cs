using System.Collections;
using UnityEngine;

namespace Character.Scripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float jumpForce = 20f;
        [SerializeField] private GameObject headGameObject;
        [SerializeField] private GameObject hairGameObject;
        [SerializeField] private bool isRigidBodyMovement = true;
        [SerializeField] private bool addWobblyMovement = false;


        private SpriteRenderer _hairSpriteRenderer;
        private SpriteRenderer _headSpriteRenderer;
        private Rigidbody2D _rigidbody2D;
        private SpriteRenderer _spriteRenderer;
        private bool _isOnGround;
        private bool _isMoving = false;
        private bool _flipEnabled = true;
        private bool _isCrawling = false;

        private void Start()
        {
            if (headGameObject) _headSpriteRenderer = headGameObject.GetComponent<SpriteRenderer>();
            if (hairGameObject) _hairSpriteRenderer = hairGameObject.GetComponent<SpriteRenderer>();

            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            if (addWobblyMovement) StartCoroutine(WobblyMovement());
        }

        private void Update()
        {
            if (isRigidBodyMovement)
                RigidBodyMovement();
            else
                CharacterControllerMovement();

            HandleHeadAndHair();
        }

        private void RigidBodyMovement()
        {
            var horizontal = Input.GetAxis("Horizontal");

            if (horizontal != 0)
            {
                var xVelocity = horizontal * speed;
                if (_isCrawling) xVelocity /= 2;

                _rigidbody2D.velocity = new Vector2(xVelocity, _rigidbody2D.velocity.y);
                if (_flipEnabled) _spriteRenderer.flipX = horizontal > 0;
                _isMoving = true;
            }
            else
            {
                _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
                _isMoving = false;
            }

            if (!_isOnGround)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                _rigidbody2D.AddForce(Vector2.up * (jumpForce), ForceMode2D.Impulse);
                //_isOnGround = false;
            }
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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                _isOnGround = true;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                _isOnGround = false;
            }
        }

        private IEnumerator WobblyMovement()
        {
            while (true)
            {
                if (_isMoving)
                {
                    var direction = Mathf.Sign(_rigidbody2D.velocity.x);
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