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
        Human
    }

    public class ObjectsAutoMove : MonoBehaviour

    {
        [SerializeField] private Direction direction;
        [SerializeField] private Speed speed;
        [SerializeField] private float stopAtX;
        [SerializeField] public bool start;

        private void Update()
        {
            if (!start) return;
            if (stopAtX != 0 && transform.position.x <= stopAtX) return;

            var currentSpeed = speed switch
            {
                Speed.Slow => 0.01f,
                Speed.Medium => 0.05f,
                Speed.Fast => 0.1f,
                Speed.Human => 5f,
                _ => 0.1f
            };

            var currentDirection = direction switch
            {
                Direction.Up => Vector3.up,
                Direction.Down => Vector3.down,
                Direction.Left => Vector3.left,
                Direction.Right => Vector3.right,
                _ => Vector3.right
            };

            transform.Translate(currentDirection * (currentSpeed * Time.deltaTime));
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.AutoMovementTrigger)
                start = true;
        }
    }
}