using UnityEngine;

namespace Hands.Scripts {
    public class KeyboardMovement : MonoBehaviour {
        [SerializeField] private float speed = 1;
        [SerializeField] private Bounds bounds;

        private void Update() {
            var newPosition = transform.position;

            if (Input.GetKey(KeyCode.W)) newPosition += Vector3.up * (speed * Time.deltaTime);
            if (Input.GetKey(KeyCode.A)) newPosition += Vector3.left * (speed * Time.deltaTime);
            if (Input.GetKey(KeyCode.S)) newPosition += Vector3.down * (speed * Time.deltaTime);
            if (Input.GetKey(KeyCode.D)) newPosition += Vector3.right * (speed * Time.deltaTime);

            // Clamp the new position within the defined bounds
            newPosition.x = Mathf.Clamp(newPosition.x, bounds.right, bounds.left);
            newPosition.y = Mathf.Clamp(newPosition.y, bounds.bottom, bounds.top);

            transform.position = newPosition;
        }
    }
}