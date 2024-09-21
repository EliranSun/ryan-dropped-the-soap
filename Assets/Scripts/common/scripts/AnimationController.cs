using UnityEngine;

namespace common.scripts
{
    [RequireComponent(typeof(Animator))]
    public class AnimationController : MonoBehaviour
    {
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        private Animator _animator;
        private bool _isWalking;

        private void Start()
        {
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