using UnityEngine;

namespace Hands.Scripts {
    public class LookAt : MonoBehaviour {
        [SerializeField] private Transform targetObject;
        [SerializeField] private bool isMirrored;

        private void Update() {
            if (targetObject != null) {
                var direction = targetObject.position - transform.position;
                var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                if (isMirrored) angle += 180f; // Invert the direction if mirrored

                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }
}