using UnityEngine;

namespace Character.Scripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Movement : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private GameObject headGameObject;
        [SerializeField] private GameObject hairGameObject;
        [SerializeField] private float jumpForce = 20f;
        private SpriteRenderer _hairSpriteRenderer;
        private SpriteRenderer _headSpriteRenderer;

        private Rigidbody2D _rigidbody2D;
        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            if (headGameObject) _headSpriteRenderer = headGameObject.GetComponent<SpriteRenderer>();
            if (hairGameObject) _hairSpriteRenderer = hairGameObject.GetComponent<SpriteRenderer>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            HandleMovement();
            HandleHeadAndHair();
        }

        private void HandleMovement()
        {
            var horizontal = Input.GetAxis("Horizontal");

            if (horizontal != 0)
                _rigidbody2D.velocity = new Vector2(horizontal * speed, _rigidbody2D.velocity.y);

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                _rigidbody2D.AddForce(Vector2.up * (speed * jumpForce), ForceMode2D.Impulse);
        }

        private void HandleHeadAndHair()
        {
            if (!headGameObject && !hairGameObject) return;

            var horizontal = Input.GetAxis("Horizontal");
            var oldHeadPosition = headGameObject ? headGameObject.transform.localPosition : Vector3.zero;
            var oldHairPosition = hairGameObject ? hairGameObject.transform.localPosition : Vector3.zero;

            switch (horizontal)
            {
                case > 0:
                    _spriteRenderer.flipX = false;
                    if (headGameObject) oldHeadPosition.x = 0.3f;
                    if (hairGameObject) oldHairPosition.x = 0.3f;
                    break;
                case < 0:
                    _spriteRenderer.flipX = true;
                    if (headGameObject) oldHeadPosition.x = -0.3f;
                    if (hairGameObject) oldHairPosition.x = -0.3f;
                    break;
                case 0:
                    if (headGameObject) oldHeadPosition.x = 0;
                    if (hairGameObject) oldHairPosition.x = 0;
                    break;
            }

            if (headGameObject) headGameObject.transform.localPosition = oldHeadPosition;
            if (hairGameObject) hairGameObject.transform.localPosition = oldHairPosition;
            if (headGameObject) _headSpriteRenderer.flipX = _spriteRenderer.flipX;
            if (hairGameObject) _hairSpriteRenderer.flipX = _spriteRenderer.flipX;
        }
    }
}