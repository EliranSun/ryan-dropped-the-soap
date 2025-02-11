using UnityEngine;

namespace npc
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class NpcScriptableMovement : MonoBehaviour
    {
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        [SerializeField] private int speed;
        [SerializeField] private Transform[] pointsOfInterest;
        [SerializeField] private float distanceToChangePoint = 4f;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float jumpForce = 20f;
        [SerializeField] private float distanceToPlayer = 4f;
        private Animator _animator;
        private int _currentPointOfInterestIndex;
        private bool _isJumping;
        private bool _isWalking;
        private Rigidbody2D _rigidBody2D;
        private SpriteRenderer _spriteRenderer;
        private Vector2 _targetPosition;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _rigidBody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            SetNextPointOfInterest();
        }

        private void FixedUpdate()
        {
            if (pointsOfInterest == null || pointsOfInterest.Length == 0 ||
                _currentPointOfInterestIndex >= pointsOfInterest.Length)
                return;

            var direction = (pointsOfInterest[0].transform.position - transform.position).normalized;
            var distance = Vector2.Distance(transform.position, pointsOfInterest[0].transform.position);
            var distanceFromPlayer = Vector2.Distance(transform.position, playerTransform.position);

            // print($"distance: {distance}, distanceFromPlayer: {distanceFromPlayer}");

            if (distance <= distanceToChangePoint)
            {
                if (_isWalking)
                {
                    _animator.SetBool(IsWalking, false);
                    _isWalking = false;
                }

                return;
            }

            if (!_isWalking)
            {
                _isWalking = true;
                _animator.SetBool(IsWalking, true);
            }

            _spriteRenderer.flipX = direction.x > 0;
            _rigidBody2D.velocityX = direction.x * speed;

            if (_isWalking && !_isJumping && Mathf.Abs(distanceFromPlayer) < distanceToPlayer)
            {
                // avoid player by jumping over them.
                // of course the optimal solution would be to prevent collision,
                // but this solution might be more fun.
                _rigidBody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                _isJumping = true;
            }

            // SetNextPointOfInterest();
            // _currentPointOfInterestIndex = (_currentPointOfInterestIndex + 1) % pointsOfInterest.Length;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Ground")) _isJumping = false;
        }

        private void SetNextPointOfInterest()
        {
            if (pointsOfInterest == null || pointsOfInterest.Length == 0) return;

            _targetPosition = pointsOfInterest[_currentPointOfInterestIndex].position;
        }
    }
}