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
        private Animator _animator;
        private int _currentPointOfInterestIndex;
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
            if (pointsOfInterest.Length == 0 || _currentPointOfInterestIndex >= pointsOfInterest.Length) return;

            var direction = (_targetPosition - (Vector2)transform.position).normalized;
            _rigidBody2D.velocityX = direction.x * speed;
            var distance = Vector2.Distance(transform.position, _targetPosition);
            _animator.SetBool(IsWalking, true);
            _spriteRenderer.flipX = direction.x < 0;

            if (distance >= distanceToChangePoint)
                return;

            _animator.SetBool(IsWalking, false);
            SetNextPointOfInterest();
            _currentPointOfInterestIndex = (_currentPointOfInterestIndex + 1) % pointsOfInterest.Length;
        }

        private void SetNextPointOfInterest()
        {
            if (pointsOfInterest.Length == 0) return;

            _targetPosition = pointsOfInterest[_currentPointOfInterestIndex].position;
        }
    }
}