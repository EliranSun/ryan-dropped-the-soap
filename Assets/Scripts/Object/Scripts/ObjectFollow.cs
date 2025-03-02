using UnityEngine;
using UnityEngine.Serialization;

namespace Object.Scripts
{
    public class ObjectFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float xOffset;
        [SerializeField] private float yOffset;

        [FormerlySerializedAs("staticZ")] [SerializeField]
        private bool lockZ;

        [SerializeField] private bool lockY;
        private float _initialZ;

        private void Start()
        {
            _initialZ = transform.position.z;
        }

        private void Update()
        {
            var newPosition = target.position;
            newPosition.y += yOffset;
            newPosition.x += xOffset;

            if (lockZ) newPosition.z = _initialZ;
            if (lockY) newPosition.y = yOffset;

            transform.position = newPosition;
        }
    }
}