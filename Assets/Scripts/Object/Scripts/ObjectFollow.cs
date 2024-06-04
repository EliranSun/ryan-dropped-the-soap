using UnityEngine;

namespace Object.Scripts {
    public class ObjectFollow : MonoBehaviour {
        [SerializeField] private Transform target;
        [SerializeField] private float xOffset;
        [SerializeField] private float yOffset;

        private void Update() {
            var newPosition = target.position;
            newPosition.y += yOffset;
            newPosition.x += xOffset;
            transform.position = newPosition;
        }
    }
}