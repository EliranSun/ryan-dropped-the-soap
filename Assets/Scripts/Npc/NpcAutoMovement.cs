using System;
using System.Collections;
using UnityEngine;

namespace Npc
{
    [Serializable]
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public class NpcAutoMovement : MonoBehaviour
    {
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");

        [SerializeField] private Vector3 direction = Vector3.left;
        [SerializeField] private bool isContinuous = true;
        [SerializeField] private float speed = 2f;
        [SerializeField] private float downwardTime = 6f;
        [SerializeField] private float upwardTime = 12f;
        private Animator _animator;
        private bool _isMovingLeft;

        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
            _animator.SetBool(IsWalking, true);

            StartCoroutine(DirectionRoutine());
        }

        private void Update()
        {
            transform.Translate(direction * (speed * Time.deltaTime), Space.World);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Walls")) FlipDirection();
        }

        private IEnumerator DirectionRoutine()
        {
            while (true)
            {
                FlipDirection();
                yield return new WaitForSeconds(_isMovingLeft ? downwardTime : upwardTime);
            }
        }

        private void FlipDirection()
        {
            _isMovingLeft = direction == Vector3.left;
            _spriteRenderer.flipX = !_isMovingLeft;
            direction = _isMovingLeft ? Vector3.right : Vector3.left;
        }
    }
}