using System;
using UnityEngine;

namespace Hands.Scripts {
    [Serializable]
    public class Bounds {
        public float top;
        public float bottom;
        public float left;
        public float right;
    }

    public class MouseMovement : MonoBehaviour {
        [SerializeField] private float speed = 1;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Bounds bounds;

        private void Update() {
            var mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = transform.position.z;

            // Clamp the mouse position within the defined bounds
            mousePosition.x = Mathf.Clamp(mousePosition.x, bounds.right, bounds.left);
            mousePosition.y = Mathf.Clamp(mousePosition.y, bounds.bottom, bounds.top);

            transform.position = Vector3.Lerp(transform.position, mousePosition, speed * Time.deltaTime);
        }
    }
}