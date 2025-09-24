using System;
using System.Collections;
using UnityEngine;

namespace Common.scripts
{
    [Serializable]
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public class CameraPan : MonoBehaviour
    {
        [SerializeField] private Direction direction;
        [SerializeField] private float speed;
        [SerializeField] private float transitionDuration = 1f;
        [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private float _stopAtY;
        private Direction _targetDirection;
        private float _targetSpeed;
        private bool _isTransitioning = false;
        private Coroutine _transitionCoroutine;

        private void Update()
        {
            if (_stopAtY != 0 && direction == Direction.Down && transform.position.y <= _stopAtY)
                return;

            if (!_isTransitioning)
            {
                MoveCamera();
            }
        }

        private void MoveCamera()
        {
            switch (direction)
            {
                case Direction.Up:
                    transform.Translate(Vector3.up * (speed * Time.deltaTime));
                    break;

                case Direction.Down:
                    transform.Translate(Vector3.down * (speed * Time.deltaTime));
                    break;

                case Direction.Left:
                    transform.Translate(Vector3.left * (speed * Time.deltaTime));
                    break;

                case Direction.Right:
                    transform.Translate(Vector3.right * (speed * Time.deltaTime));
                    break;
            }
        }

        private IEnumerator SmoothTransition(Direction newDirection, float newSpeed)
        {
            _isTransitioning = true;

            Direction startDirection = direction;
            float startSpeed = speed;
            float elapsedTime = 0f;

            while (elapsedTime < transitionDuration)
            {
                float t = elapsedTime / transitionDuration;
                float curveValue = transitionCurve.Evaluate(t);

                // Smoothly interpolate speed
                speed = Mathf.Lerp(startSpeed, newSpeed, curveValue);

                // For direction change, we'll gradually reduce speed to 0, change direction, then ramp up
                if (startDirection != newDirection)
                {
                    if (t < 0.5f)
                    {
                        // First half: slow down current direction
                        speed = Mathf.Lerp(startSpeed, 0, curveValue * 2);
                    }
                    else
                    {
                        // Second half: change direction and speed up
                        if (direction != newDirection)
                        {
                            direction = newDirection;
                        }
                        speed = Mathf.Lerp(0, newSpeed, (curveValue - 0.5f) * 2);
                    }
                }

                // Continue moving during transition
                MoveCamera();

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure final values are set
            direction = newDirection;
            speed = newSpeed;
            _isTransitioning = false;
        }

        public void GoToGroundFloor()
        {
            _stopAtY = 1;
            StartSmoothTransition(Direction.Down, 25f);
        }

        public void StartSmoothTransition(Direction newDirection, float newSpeed)
        {
            // Stop any existing transition
            if (_transitionCoroutine != null)
            {
                StopCoroutine(_transitionCoroutine);
            }

            _transitionCoroutine = StartCoroutine(SmoothTransition(newDirection, newSpeed));
        }

        public void SetTransitionDuration(float duration)
        {
            transitionDuration = Mathf.Max(0.1f, duration);
        }

        public void SetTransitionCurve(AnimationCurve curve)
        {
            transitionCurve = curve ?? AnimationCurve.EaseInOut(0, 0, 1, 1);
        }

        public bool IsTransitioning => _isTransitioning;
    }
}