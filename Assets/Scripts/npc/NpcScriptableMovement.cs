using System;
using UnityEngine;

namespace npc
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class NpcScriptableMovement : MonoBehaviour
    {
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");

        // if PoI is player, speed should be lower than player speed to avoid animation jitter
        // (because the NPC constantly reaching and stopping)
        [SerializeField] private float speed;
        [SerializeField] private Transform[] pointsOfInterest;
        [SerializeField] private float distanceToChangePoint = 4f;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float jumpForce = 20f;
        [SerializeField] private float distanceToPlayer = 4f;
        [SerializeField] private bool isRigidBodyMovement = true;

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

            var distance = Vector2.Distance(transform.position, pointsOfInterest[0].transform.position);

            // print($"distance: {distance}, distanceFromPlayer: {distanceFromPlayer}");

            if (distance <= distanceToChangePoint)
            {
                // reached point
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

            HandleMovement();
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

        private void HandleMovement()
        {
            var direction = (pointsOfInterest[0].transform.position - transform.position).normalized;
            var distanceFromPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (isRigidBodyMovement)
                RigidBodyMovement(direction, distanceFromPlayer);
            else
                TransformMovement(direction, distanceFromPlayer);
        }

        private void RigidBodyMovement(Vector2 direction, float distanceFromPlayer)
        {
            _spriteRenderer.flipX = direction.x > 0;
            _rigidBody2D.linearVelocityX = direction.x * speed;

            if (_isWalking && !_isJumping && Mathf.Abs(distanceFromPlayer) < distanceToPlayer)
            {
                // avoid player by jumping over them.
                // of course the optimal solution would be to prevent collision,
                // but this solution might be more fun.
                _rigidBody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                _isJumping = true;
            }
        }

        private void TransformMovement(Vector2 direction, float distanceFromPlayer)
        {
            transform.Translate(new Vector3(direction.x, 0, 0) * (speed * Time.deltaTime));
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.FollowPlayer)
                if (playerTransform != null)
                    AddPointOfInterest(playerTransform);

            if (eventData.Name == GameEvents.NpcGoTo)
            {
                var pointOfInterest = (GameObject)eventData.Data;
                if (pointOfInterest != null)
                    ReplacePointOfInterest(pointOfInterest.transform);
            }
        }

        private void AddPointOfInterest(Transform newPointOfInterest)
        {
            // Create a new array with one more element
            var newPointsOfInterest = new Transform[pointsOfInterest != null ? pointsOfInterest.Length + 1 : 1];

            // Copy existing points of interest if any
            if (pointsOfInterest is { Length: > 0 })
                Array.Copy(pointsOfInterest, newPointsOfInterest, pointsOfInterest.Length);

            // Add player transform as the last point of interest
            newPointsOfInterest[^1] = newPointOfInterest;

            UpdatePointsOfInterest(newPointsOfInterest);
        }

        private void ReplacePointOfInterest(Transform newPointOfInterest)
        {
            // Create a new array with just the new point
            var newPointsOfInterest = new[] { newPointOfInterest };
            UpdatePointsOfInterest(newPointsOfInterest);
        }

        private void UpdatePointsOfInterest(Transform[] newPointsOfInterest)
        {
            // Update the points of interest array
            pointsOfInterest = newPointsOfInterest;

            // Reset current index to ensure we start following
            _currentPointOfInterestIndex = 0;

            // Set the target position to the player
            SetNextPointOfInterest();
        }
    }
}