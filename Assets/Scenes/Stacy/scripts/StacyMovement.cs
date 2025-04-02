using UnityEngine;

namespace Scenes.Stacy.scripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class StacyMovement : MonoBehaviour
    {
        [SerializeField] private float movementSpeed = 5f;
        [SerializeField] private float jumpForce = 50f;
        [SerializeField] private float maxSpeed = 8f;
        private Rigidbody2D _rigidbody2D;
        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            var moveRight = Input.GetKeyDown(KeyCode.D);
            var moveLeft = Input.GetKeyDown(KeyCode.A);
            var jump = Input.GetKeyDown(KeyCode.W);

            var releaseRight = Input.GetKeyUp(KeyCode.D);
            var releaseLeft = Input.GetKeyUp(KeyCode.A);

            // Clamp horizontal velocity to max speed
            var currentVelocity = _rigidbody2D.velocity;
            currentVelocity.x = Mathf.Clamp(currentVelocity.x, -maxSpeed, maxSpeed);
            _rigidbody2D.velocity = currentVelocity;
            
            // TODO: Move sprite to inner game object to avoid rotation issues
            // rotate is just cosmetic, forces are applied to the rigidbody
            
            if (moveRight) {
                _rigidbody2D.AddForce(Vector2.right * movementSpeed, ForceMode2D.Impulse);
                transform.Rotate(0, 0, -5);
                _spriteRenderer.flipX = false;
            }

            if (moveLeft) {
                _rigidbody2D.AddForce(Vector2.left * movementSpeed, ForceMode2D.Impulse);
                transform.Rotate(0, 0, 5);
                _spriteRenderer.flipX = true;
            }

            if (jump) _rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            if (releaseRight || releaseLeft) {
                transform.rotation = Quaternion.identity;
            }
        }
    }
}