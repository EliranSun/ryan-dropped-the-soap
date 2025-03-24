using UnityEngine;

namespace Mini_Games
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class WobblyMovement : ObserverSubject
    {
        [Header("References")] [SerializeField]
        private Transform playerTransform;

        [SerializeField] private Rigidbody2D playerRigidbody;

        [Header("Movement Settings")] [SerializeField]
        public float moveSpeed = 5f;

        [SerializeField] private float jumpForce = 5f;
        private bool _hasMoved;
        private bool _isFacingRight = true;
        private SpriteRenderer _spriteRenderer;
        private bool _inAir = false;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _isFacingRight = _spriteRenderer.flipX;
        }

        private void Update()
        {
            var moveRight = Input.GetKeyDown(KeyCode.D);
            var moveLeft = Input.GetKeyDown(KeyCode.A);

            if (moveRight) PushForward(Vector2.right);
            else if (moveLeft) PushForward(Vector2.left);

            // Reset rotation when keys are released
            if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A))
                transform.rotation = Quaternion.identity;
        }

        private void PushForward(Vector2 direction)
        {
            if (!_hasMoved)
            {
                Notify(GameEvents.FirstTimePlayerMove);
                _hasMoved = true;
            }

            // reset rotation
            transform.rotation = Quaternion.identity;

            FlipSprite(direction);

            if (_inAir) return;

            playerRigidbody.velocity = Vector2.zero;
            playerRigidbody.AddForce(direction * moveSpeed + Vector2.up * jumpForce, ForceMode2D.Impulse);

            // rotate object angle a bit in the direction of movement
            transform.Rotate(Vector3.forward, direction.x * -5f);
        }

        private void FlipSprite(Vector2 direction)
        {
            if (_isFacingRight && direction == Vector2.left)
            {
                _spriteRenderer.flipX = false;
                _isFacingRight = false;
            }
            else if (!_isFacingRight && direction == Vector2.right)
            {
                _spriteRenderer.flipX = true;
                _isFacingRight = true;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                _inAir = false;
            }
        }


        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                _inAir = true;
            }
        }
    }
}