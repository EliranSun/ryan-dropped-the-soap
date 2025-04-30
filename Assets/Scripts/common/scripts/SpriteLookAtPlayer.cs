using UnityEngine;

namespace common.scripts
{
    public class SpriteLookAtPlayer : MonoBehaviour
    {
        [SerializeField] private Transform playerTransform;
        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            _spriteRenderer.flipX = transform.position.x < playerTransform.position.x;
        }
    }
}