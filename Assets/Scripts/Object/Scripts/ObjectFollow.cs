using UnityEngine;

namespace Object.Scripts
{
    public class ObjectFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float xOffset;
        [SerializeField] private float yOffset;
        [SerializeField] private bool staticZ;
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
            if (staticZ) newPosition.z = _initialZ;

            transform.position = newPosition;
        }
    }
}