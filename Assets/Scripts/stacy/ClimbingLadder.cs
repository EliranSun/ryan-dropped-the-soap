using Player;
using UnityEngine;

namespace stacy
{
    public class ClimbingLadder : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private BoxCollider2D ladderTrigger;

        [SerializeField] private float ladderWidth = 1f;
        [SerializeField] private float ladderTop;
        [SerializeField] private float ladderBottom;

        [Header("Movement Settings")] [SerializeField]
        private float climbSpeed = 3f;

        [SerializeField] private float horizontalMoveSpeed = 2f;
        [SerializeField] private float maxHorizontalOffset = 0.4f;
        private bool _isPlayerOnLadder;
        private Vector2 _ladderCenter;
        private float _originalGravity;

        private GameObject _player;
        private Rigidbody2D _playerRb;

        // Start is called before the first frame update
        private void Start()
        {
            if (ladderTrigger == null) ladderTrigger = GetComponent<BoxCollider2D>();

            if (ladderTrigger != null)
            {
                _ladderCenter = transform.position;
                // Calculate ladder top and bottom if not set in inspector
                if (ladderTop == 0 && ladderBottom == 0)
                {
                    var bounds = ladderTrigger.bounds;
                    ladderTop = bounds.max.y;
                    ladderBottom = bounds.min.y;
                }
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (_isPlayerOnLadder && _player != null)
            {
                // Disable gravity while on ladder
                _playerRb.gravityScale = 0;

                // Get input for climbing
                float verticalInput = 0;
                if (Input.GetKey(KeyCode.W))
                    verticalInput = 1;
                else if (Input.GetKey(KeyCode.S))
                    verticalInput = -1;

                float horizontalInput = 0;
                if (Input.GetKey(KeyCode.D))
                    horizontalInput = 1;
                else if (Input.GetKey(KeyCode.A))
                    horizontalInput = -1;

                // Calculate movement
                var movement = new Vector2(horizontalInput * horizontalMoveSpeed, verticalInput * climbSpeed);

                // Apply movement
                var newPosition = _playerRb.position + movement * Time.deltaTime;

                // Check vertical boundaries
                newPosition.y = Mathf.Clamp(newPosition.y, ladderBottom, ladderTop);

                // Check horizontal boundaries and drop if too far
                if (Mathf.Abs(newPosition.x - _ladderCenter.x) > maxHorizontalOffset)
                {
                    DropFromLadder();
                    return;
                }

                // Move player
                _playerRb.linearVelocity = Vector2.zero;
                _playerRb.MovePosition(newPosition);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                _player = collision.gameObject;
                _playerRb = _player.GetComponent<Rigidbody2D>();
                if (_playerRb != null)
                {
                    _originalGravity = _playerRb.gravityScale;
                    _isPlayerOnLadder = true;
                    // Disable player's other movement components temporarily
                    var movement = _player.GetComponent<Movement>();
                    if (movement != null) movement.enabled = false;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && _player == collision.gameObject) DropFromLadder();
        }

        private void DropFromLadder()
        {
            if (_player != null && _playerRb != null)
            {
                _isPlayerOnLadder = false;
                // Restore gravity
                _playerRb.gravityScale = _originalGravity;
                // Re-enable player's movement component
                var movement = _player.GetComponent<Movement>();
                if (movement != null) movement.enabled = true;
            }
        }
    }
}