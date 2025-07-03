using UnityEngine;

namespace Dropped_The_Soap.Scripts
{
    public class LookAt : MonoBehaviour
    {
        [SerializeField] private Transform targetObject;
        [SerializeField] private bool isMirrored;

        private void Update()
        {
            if (!targetObject) return;

            var direction = targetObject.position - transform.position;
            // var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // if (isMirrored) angle += 180f; // Invert the direction if mirrored

            print($"Target object Y rotation: {RotationYToAngle(targetObject.transform.rotation.y)}");
            transform.rotation = Quaternion.Euler(0, RotationYToAngle(targetObject.transform.rotation.y), 0);
        }

        private float RotationYToAngle(float rotationY)
        {
            var angle = Mathf.Atan2(rotationY, 1) * Mathf.Rad2Deg;
            return angle;
        }
    }
}