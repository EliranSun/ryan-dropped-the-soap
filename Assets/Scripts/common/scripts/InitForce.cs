using System;
using UnityEngine;

namespace common.scripts
{
    [Serializable]
    internal enum Force
    {
        Soft,
        Medium,
        Hard
    }


    [RequireComponent(typeof(Rigidbody2D))]
    public class InitForce : MonoBehaviour

    {
        [SerializeField] private float delayForce;
        [SerializeField] private Force force;
        [SerializeField] private Direction direction;
        private Vector3 _directionValue;
        private float _forceValue;
        private Rigidbody2D _rigidbody;


        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _forceValue = force switch
            {
                Force.Soft => 1f,
                Force.Medium => 3f,
                _ => 5f
            };
            _directionValue = direction switch
            {
                Direction.Up => Vector3.up,

                Direction.Down => Vector3.down,
                Direction.Left => Vector3.left,
                _ => Vector3.right
            };

            Invoke(nameof(AddForce), delayForce);
        }

        private void AddForce()
        {
            _rigidbody.AddForce(_directionValue * _forceValue, ForceMode2D.Impulse);
        }
    }
}