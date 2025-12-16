using System;
using UnityEngine;

namespace common.scripts
{
    [Serializable]
    internal enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    [Serializable]
    internal enum Speed
    {
        Slow,
        Medium,
        Fast,
        Human,
        PanImageSlow
    }

    public class ObjectPan : ObserverSubject

    {
        [SerializeField] private Direction direction;
        [SerializeField] private Speed speed;
        [SerializeField] private float stopAtX;
        [SerializeField] private bool shouldStopAtX;
        [SerializeField] public bool start;
        [SerializeField] public bool startOnEnable;
        [SerializeField] public float reachTargetOnTime;
        [SerializeField] private GameEvents onReachStopEvent;

        private float _currentSpeed;
        private float _halfWidth;

        private void Start()
        {
            // Cache half-width so we can stop the element when its right edge reaches stopAtX.
            _halfWidth = CalculateHalfWidth();
            _currentSpeed = reachTargetOnTime > 0
                ? CalculateSpeedBasedOnTime()
                : speed switch
                {
                    Speed.Slow => 0.01f,
                    Speed.Medium => 0.05f,
                    Speed.Fast => 0.1f,
                    Speed.Human => 5f,
                    Speed.PanImageSlow => 50f,
                    _ => 0.1f
                };
        }

        private void Update()
        {
            var reachedStop = false;
            if (!start)
                return;

            if (shouldStopAtX)
            {
                if (direction == Direction.Left)
                    reachedStop = transform.position.x <= stopAtX;
                if (direction == Direction.Right)
                    reachedStop = transform.position.x >= stopAtX;


                if (reachedStop)
                {
                    Notify(onReachStopEvent);
                    return;
                }
            }

            // print($"{gameObject.name} DIR {direction}; x: {transform.position.x}; stop at: {stopAtX}");

            var currentDirection = direction switch
            {
                Direction.Up => Vector3.up,
                Direction.Down => Vector3.down,
                Direction.Left => Vector3.left,
                Direction.Right => Vector3.right,
                _ => Vector3.right
            };

            transform.Translate(currentDirection * (_currentSpeed * Time.deltaTime));
            // }
        }

        private void OnEnable()
        {
            if (startOnEnable) start = true;
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.AutoMovementTrigger)
                start = true;
        }

        private float CalculateSpeedBasedOnTime()
        {
            // Calculate horizontal speed (units per second) needed to bring the right edge to stopAtX in reachTargetOnTime
            var remaining = Mathf.Abs(stopAtX - transform.position.x);
            return remaining / reachTargetOnTime;
        }

        private float CalculateHalfWidth()
        {
            if (transform is RectTransform rectTransform)
                // rect size is in local space; scale by lossyScale for world units
                return rectTransform.rect.width * 0.5f * rectTransform.lossyScale.x;

            // Fallback: try renderer bounds, otherwise zero
            if (TryGetComponent(out Renderer renderer)) return renderer.bounds.size.x * 0.5f;

            return 0f;
        }
    }
}