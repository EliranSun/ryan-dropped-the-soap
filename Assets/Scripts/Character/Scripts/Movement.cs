using UnityEngine;

namespace Character.Scripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        private Rigidbody2D _rigidbody2D;
        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            _rigidbody2D.velocity = new Vector2(horizontal * speed, vertical * speed);

            if (horizontal > 0)
                _spriteRenderer.flipX = false;
            else if (horizontal < 0)
                _spriteRenderer.flipX = true;
        }
    }
}