using System;
using Character_Creator.scripts;
using Dialog;
using Elevator.scripts;
using UnityEngine;

namespace npc
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class NpcScriptableMovement : ObserverSubject
    {
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");

        // if PoI is player, speed should be lower than player speed to avoid animation jitter
        // (because the NPC constantly reaching and stopping)
        [SerializeField] private float speed;
        [SerializeField] private float distanceToChangePoint = 4f;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float jumpForce = 20f;
        [SerializeField] private float distanceToPlayer = 4f;
        [SerializeField] private bool isRigidBodyMovement = true;
        [SerializeField] private bool avoidPlayer;
        [SerializeField] private bool loopPointsOfInterest = true;
        [SerializeField] private ActorName actorName;
        [SerializeField] private ApartmentsController apartmentsController;

        [Header("Wobbly Movement")]
        [SerializeField]
        private bool isMovementWobbly;

        [SerializeField] private float wobbleAmplitudeDegrees = 7f; // max lean angle
        [SerializeField] private float wobbleFrequency = 1f; // cycles per second

        [Header("Points of interest")]
        [SerializeField]
        private Transform[] pointsOfInterest;

        private Animator _animator;
        private int _currentPointOfInterestIndex;
        private bool _isJumping;
        private bool _isWalking;
        private Rigidbody2D _rigidBody2D;
        private SpriteRenderer _spriteRenderer;
        private Vector2 _targetPosition;
        private float _wobbleTime;

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

            var currentPoint = pointsOfInterest[_currentPointOfInterestIndex];
            var distance = Vector2.Distance(transform.position, currentPoint.position);

            if (distance <= distanceToChangePoint)
            {
                // reached point
                if (_isWalking)
                {
                    _animator.SetBool(IsWalking, false);
                    _isWalking = false;
                    _currentPointOfInterestIndex++;

                    if (loopPointsOfInterest)
                        _currentPointOfInterestIndex %= pointsOfInterest.Length;
                    else if (_currentPointOfInterestIndex >= pointsOfInterest.Length)
                        // _currentPointOfInterestIndex = pointsOfInterest.Length - 1;
                        return;

                    SetNextPointOfInterest();
                    Notify(GameEvents.NpcAtPointOfInterest);
                }

                return;
            }

            if (!_isWalking)
            {
                _isWalking = true;
                _animator.SetBool(IsWalking, true);
            }

            HandleMovement();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Ground")) _isJumping = false;
        }

        private void WobbleForward(Vector2 direction)
        {
            // Flip sprite based on direction
            _spriteRenderer.flipX = direction.x > 0;

            // Advance wobble timer
            _wobbleTime += Time.deltaTime;

            // Compute wobble angle: oscillate between straight (0) and leaning forward (+/- amplitude)
            // Use Abs(sin) so it goes: straight -> lean -> straight -> lean ... in the move direction
            var leanSign = direction.x >= 0 ? -1f : 1f; // keep same handedness as previous implementation
            var wobblePhase = Mathf.Abs(Mathf.Sin(_wobbleTime * Mathf.PI * 2f * wobbleFrequency));
            var angleZ = leanSign * wobbleAmplitudeDegrees * wobblePhase;

            // Apply explicit rotation (no accumulation) and move forward
            transform.rotation = Quaternion.Euler(0f, 0f, angleZ);
            transform.Translate(new Vector3(direction.x, 0f, 0f) * (speed * Time.deltaTime));
        }

        private void SetNextPointOfInterest()
        {
            if (pointsOfInterest == null || pointsOfInterest.Length == 0) return;

            _targetPosition = pointsOfInterest[_currentPointOfInterestIndex].position;
        }

        private void HandleMovement()
        {
            var direction = (_targetPosition - (Vector2)transform.position).normalized;

            if (isMovementWobbly) WobbleForward(direction);
            else if (isRigidBodyMovement) RigidBodyMovement(direction);
            else TransformMovement(direction);
        }

        private void RigidBodyMovement(Vector2 direction)
        {
            _spriteRenderer.flipX = direction.x > 0;
            _rigidBody2D.linearVelocityX = direction.x * speed;

            if (avoidPlayer && _isWalking && !_isJumping)
            {
                var distanceFromPlayer = Vector2.Distance(transform.position, playerTransform.position);
                if (Mathf.Abs(distanceFromPlayer) < distanceToPlayer)
                {
                    // avoid player by jumping over them.
                    // of course, the optimal solution would be to prevent collision,
                    // but this solution might be more fun.
                    _rigidBody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    _isJumping = true;
                }
            }
        }

        private void TransformMovement(Vector2 direction)
        {
            _spriteRenderer.flipX = direction.x < 0;
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

            if (eventData.Name == GameEvents.NpcGoToApartment)
            {
                var dialogProperties = DialogHelper.GetDialogNotificationProperties(eventData);
                if (dialogProperties.ActorName == actorName)
                {
                    // TODO: I don't like that npc scriptable movement which is specific per NPC 
                    // knows about apartment controller, but we need a callback here
                    var door = apartmentsController.FindNpcDoor(dialogProperties.ActionNumber);
                    AddPointOfInterest(door.gameObject.transform);
                }
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