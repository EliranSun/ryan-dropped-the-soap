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

    public class CameraPan : ObserverSubject
    {
        [SerializeField] private Direction direction;
        [SerializeField] private float speed;
        [SerializeField] private float transitionDuration = 1f;
        [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private float _stopAtY;
        private Direction _targetDirection;
        private float _targetSpeed;
        private Coroutine _transitionCoroutine;

        public bool IsTransitioning { get; private set; }

        private void Update()
        {
            if (_stopAtY == 0)
                return;

            if (direction == Direction.Down && transform.position.y <= _stopAtY)
                return;

            if (direction == Direction.Up && transform.position.y >= _stopAtY)
                return;

            if (!IsTransitioning)
                MoveCamera();
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
            IsTransitioning = true;

            var startDirection = direction;
            var startSpeed = speed;
            var elapsedTime = 0f;

            while (elapsedTime < transitionDuration)
            {
                var t = elapsedTime / transitionDuration;
                var curveValue = transitionCurve.Evaluate(t);

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
                        if (direction != newDirection) direction = newDirection;
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
            IsTransitioning = false;
            Notify(GameEvents.CameraTransitionEnded);
        }

        public void GoToGroundFloor()
        {
            _stopAtY = 1;
            StartSmoothTransition(Direction.Down, 25f);
        }

        public void StartSmoothTransition(Direction newDirection, float newSpeed)
        {
            // Stop any existing transition
            if (_transitionCoroutine != null) StopCoroutine(_transitionCoroutine);

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

        /// <summary>
        ///     Skips the camera pan entirely and gives the player immediate control.
        ///     Useful for testing or when you want to bypass the intro camera animation.
        /// </summary>
        /// <param name="player">The player GameObject to center the camera on</param>
        public void SkipCameraPanAndEnablePlayer(GameObject player)
        {
            if (player == null)
            {
                Debug.LogWarning("Player GameObject is null. Cannot skip camera pan.");
                return;
            }

            // Reset transition state
            IsTransitioning = false;
            _stopAtY = 0;

            // Stop any existing transition
            if (_transitionCoroutine != null)
            {
                StopCoroutine(_transitionCoroutine);
                _transitionCoroutine = null;
            }


            // Position camera at player's center
            var playerPosition = player.transform.position;
            transform.position = new Vector3(playerPosition.x, playerPosition.y, transform.position.z);

            // Parent camera to player for follow behavior
            transform.parent = player.transform;

            // Notify that player movement should be enabled
            Notify(GameEvents.EnablePlayerMovement);

            // Also notify that camera transition ended (in case other systems are listening)
            Notify(GameEvents.CameraTransitionEnded);

            Debug.Log("Camera pan skipped - Player control enabled immediately");
        }
    }
}