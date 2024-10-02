using UnityEngine;

namespace common.scripts
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class AnimationController : MonoBehaviour
    {
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        private Animator _animator;
        private BoxCollider2D _boxCollider;
        private bool _isWalking;
        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (Input.GetAxis("Horizontal") != 0 && !_isWalking)
            {
                _isWalking = true;
                _animator.SetBool(IsWalking, true);
            }
            else if (Input.GetAxis("Horizontal") == 0 && _isWalking)
            {
                _isWalking = false;
                _animator.SetBool(IsWalking, false);
            }
        }
    }
}