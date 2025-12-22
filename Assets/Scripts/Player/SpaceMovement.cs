using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SpaceMovement : MonoBehaviour
    {
        [SerializeField] private float initialTorque = 100;
        [SerializeField] private float force;
        private Rigidbody2D _rigidbody;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _rigidbody.AddTorque(initialTorque);
        }

        private void Update()
        {
            var vectorForce = new Vector2(0, 0);

            if (Input.GetAxis("Horizontal") > 0) vectorForce.x = force;
            if (Input.GetAxis("Horizontal") < 0) vectorForce.x = -force;
            if (Input.GetAxis("Vertical") > 0) vectorForce.y = force;
            if (Input.GetAxis("Vertical") < 0) vectorForce.y = -force;

            _rigidbody.AddForce(vectorForce, ForceMode2D.Impulse);
        }
    }
}