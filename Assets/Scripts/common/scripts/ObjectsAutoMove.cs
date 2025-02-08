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
        Fast
    }

    public class ObjectsAutoMove : MonoBehaviour

    {
        [SerializeField] private Direction direction;
        [SerializeField] private Speed speed;


        private void Update()
        {
            var speed = this.speed switch
            {
                Speed.Slow => 0.01f,
                Speed.Medium => 0.05f,
                Speed.Fast => 0.1f,
                _ => 0.1f
            };

            var direction = this.direction switch
            {
                Direction.Up => Vector3.up,
                Direction.Down => Vector3.down,
                Direction.Left => Vector3.left,
                Direction.Right => Vector3.right,
                _ => Vector3.right
            };

            transform.Translate(direction * (speed * Time.deltaTime));
        }
    }
}