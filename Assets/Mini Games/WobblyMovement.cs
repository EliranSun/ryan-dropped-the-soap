using UnityEngine;

namespace Mini_Games
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class WobblyMovement : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private Transform playerTransform;

        [SerializeField] private Rigidbody2D playerRigidbody;

        [Header("Movement Settings")] [SerializeField]
        private float moveSpeed = 5f;

        [SerializeField] private float jumpForce = 5f;

        private bool _isPressed;

        private void Start()
        {
        }

        private void Update()
        {
            var moveInput = Input.GetAxis("Horizontal");

            if (moveInput > 0)
                PushForward(Vector2.right);
            else if (moveInput < 0)
                PushForward(Vector2.left);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Ground"))
                _isPressed = false;
        }

        private void PushForward(Vector2 direction)
        {
            if (_isPressed)
                return;

            playerRigidbody.velocity = Vector2.zero;
            playerRigidbody.AddForce(direction * moveSpeed + Vector2.up * jumpForce, ForceMode2D.Impulse);
            _isPressed = true;
        }
    }
}