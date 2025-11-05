using UnityEngine;

namespace common.scripts
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteLookAtPlayer : MonoBehaviour
    {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private bool facingRightByDefault;
        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (!playerTransform)
                return;

            // Ensure the sprite always faces the player,
            // but don't flip if the player is vertically aligned (edge case)
            var dir = playerTransform.position.x - transform.position.x;
            if (Mathf.Abs(dir) > 0.01f) // allow a small threshold to avoid flipping at exact alignments
                // If sprites are authored facing right (default), flip when player is left (dir < 0).
                // If authored facing left, invert the rule so they still look at the player correctly.
                _spriteRenderer.flipX = facingRightByDefault ? dir < 0f : dir > 0f;
        }
    }
}