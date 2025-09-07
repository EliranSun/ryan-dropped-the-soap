using UnityEngine;

namespace Common.scripts
{
    public class LadderMovement : MonoBehaviour
    {
        private const float VerticalClimbSpeed = 3f;
        [SerializeField] private Rigidbody2D rigidbody2D;
        private bool _isOnLadder;
        private float _originalGravityScale;

        private void Start()
        {
            _originalGravityScale = rigidbody2D.gravityScale;
        }

        private void Update()
        {
            var vertical = Input.GetAxis("Vertical");
            if (_isOnLadder && Mathf.Abs(vertical) > 0f)
                transform.position += new Vector3(0f, vertical * VerticalClimbSpeed, 0f);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Ladder"))
            {
                print("ON LADDER");
                _isOnLadder = true;
                // rigidbody2D.gravityScale = 0f;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Ladder"))
            {
                print("OFF LADDER");
                _isOnLadder = false;
                // rigidbody2D.gravityScale = _originalGravityScale;
            }
        }
    }
}