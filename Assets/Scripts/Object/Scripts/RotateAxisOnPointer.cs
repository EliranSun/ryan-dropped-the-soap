using UnityEngine;
using UnityEngine.InputSystem;

namespace Object.Scripts
{
    public class RotateAxisOnPointer : MonoBehaviour
    {
        [SerializeField] private float radius = 1f; // Distance from center
        [SerializeField] private Transform centerPoint; // Reference point to rotate around (usually player)

        private readonly float[] _angles =
        {
            90, // N
            45, // NE
            0, // E
            315, // SE
            270, // S
            225, // SW
            180, // W
            135 // NW
        };

        private readonly Vector2[] _directions =
        {
            new(0, 1), // N
            new(1, 1), // NE
            new(1, 0), // E
            new(1, -1), // SE
            new(0, -1), // S
            new(-1, -1), // SW
            new(-1, 0), // W
            new(-1, 1) // NW
        };

        private InputAction _lookAction;

        private void Start()
        {
            _lookAction = InputSystem.actions.FindAction("Look");

            if (centerPoint == null)
            {
                centerPoint = transform.parent;
                if (centerPoint == null)
                    Debug.LogWarning("No center point assigned and no parent found. Using current position as center.");
            }
        }

        private void Update()
        {
            var lookValue = _lookAction.ReadValue<Vector2>();
            if (lookValue.sqrMagnitude < 0.01f) return; // Ignore very small inputs

            // Find the closest direction
            var closestIndex = 0;
            var closestAngle = float.MaxValue;

            for (var i = 0; i < _directions.Length; i++)
            {
                var angle = Vector2.Angle(lookValue, _directions[i]);
                if (angle < closestAngle)
                {
                    closestAngle = angle;
                    closestIndex = i;
                }
            }

            // Position the object
            var newPosition = (centerPoint != null ? centerPoint.position : transform.position) +
                              new Vector3(_directions[closestIndex].x, _directions[closestIndex].y, 0) * radius;
            transform.position = newPosition;

            // Rotate the object
            transform.rotation = Quaternion.Euler(0, 0, _angles[closestIndex]);
        }
    }
}