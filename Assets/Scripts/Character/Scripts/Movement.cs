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
            _headSpriteRenderer = headGameObject.GetComponent<SpriteRenderer>();
            _hairSpriteRenderer = hairGameObject.GetComponent<SpriteRenderer>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            var horizontal = Input.GetAxis("Horizontal");
            _rigidbody2D.velocity = new Vector2(horizontal * speed, _rigidbody2D.velocity.y);

            var oldHeadPosition = headGameObject.transform.localPosition;
            var oldHairPosition = hairGameObject.transform.localPosition;

            switch (horizontal)
            {
                case > 0:
                    _spriteRenderer.flipX = false;
                    oldHeadPosition.x = 0.3f;
                    oldHairPosition.x = 0.3f;
                    break;
                case < 0:
                    _spriteRenderer.flipX = true;
                    oldHeadPosition.x = -0.3f;
                    oldHairPosition.x = -0.3f;
                    break;
                case 0:
                    oldHeadPosition.x = 0;
                    oldHairPosition.x = 0;
                    break;
            }


            headGameObject.transform.localPosition = oldHeadPosition;
            hairGameObject.transform.localPosition = oldHairPosition;
            _headSpriteRenderer.flipX = _spriteRenderer.flipX;
            _hairSpriteRenderer.flipX = _spriteRenderer.flipX;

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                _rigidbody2D.AddForce(Vector2.up * (speed * jumpForce), ForceMode2D.Impulse);
        }
    }
}