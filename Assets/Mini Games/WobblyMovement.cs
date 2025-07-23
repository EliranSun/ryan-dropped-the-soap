using UnityEngine;

namespace Mini_Games
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class WobblyMovement : ObserverSubject
    {
        [Header("References")]
        [SerializeField]
        private Transform playerTransform;

        [SerializeField] private Rigidbody2D playerRigidbody;

        [Header("Movement Settings")]
        [SerializeField]
        public float moveSpeed = 5f;

        [SerializeField] private float jumpForce = 5f;
        private bool _isOnGround = false;
        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            var moveRight = Input.GetKeyDown(KeyCode.D);
            var moveLeft = Input.GetKeyDown(KeyCode.A);
            var jump = Input.GetKeyDown(KeyCode.W);

            var movementVector = Vector2.zero;

            if (moveRight) movementVector = PushForward(Vector2.right, movementVector);
            else if (moveLeft) movementVector = PushForward(Vector2.left, movementVector);
            if (jump) movementVector = Jump(movementVector);

            // Reset rotation when keys are released
            if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A))
                transform.rotation = Quaternion.identity;

            if (movementVector != Vector2.zero)
            {
                playerRigidbody.AddForce(movementVector, ForceMode2D.Impulse);
                _isOnGround = false;
            }
        }

        private Vector2 Jump(Vector2 movementVector)
        {
            if (!_isOnGround) return movementVector;

            movementVector = Vector2.up * jumpForce * 2;
            return movementVector;
        }

        private Vector2 PushForward(Vector2 direction, Vector2 movementVector)
        {
            // Flip sprite based on direction
            _spriteRenderer.flipX = direction.x > 0;

            if (!_isOnGround) return movementVector;

            movementVector = direction * moveSpeed + Vector2.up * jumpForce;

            // rotate object angle a bit in the direction of movement
            transform.Rotate(Vector3.forward, direction.x * -5f);

            return movementVector;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Collectible Item"))
            {
                _isOnGround = true;
            }
        }
    }
}