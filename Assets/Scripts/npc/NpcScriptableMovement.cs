using UnityEngine;

namespace npc
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class NpcScriptableMovement : MonoBehaviour
    {
        [SerializeField] private int speed;
        [SerializeField] private Transform[] pointsOfInterest;
        private int _currentPointOfInterestIndex;
        private Rigidbody2D _rigidbody2D;
        private Vector2 _targetPosition;

        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            SetNextPointOfInterest();
        }

        private void FixedUpdate()
        {
            if (pointsOfInterest.Length == 0 || _currentPointOfInterestIndex >= pointsOfInterest.Length)
                return;

            var direction = (_targetPosition - (Vector2)transform.position).normalized;
            _rigidbody2D.AddForce(direction * (speed * Time.fixedDeltaTime), ForceMode2D.Force);

            if (Vector2.Distance(transform.position, _targetPosition) < 2f)
            {
                SetNextPointOfInterest();
                _currentPointOfInterestIndex = (_currentPointOfInterestIndex + 1) % pointsOfInterest.Length;
            }
        }

        private void SetNextPointOfInterest()
        {
            if (pointsOfInterest.Length == 0) return;

            _targetPosition = pointsOfInterest[_currentPointOfInterestIndex].position;
        }
    }
}