using UnityEngine;
using UnityEngine.Serialization;

namespace camera.scripts
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private GameObject player;

        [FormerlySerializedAs("elevator")] [SerializeField]
        private GameObject target;

        private Transform _elevatorTransform;
        private Transform _playerTransform;

        private void Start()
        {
            _playerTransform = player.transform;
            _elevatorTransform = target.transform;
        }

        private void Update()
        {
            var distance = Vector2.Distance(_playerTransform.position, _elevatorTransform.position);
            print(distance);


            if (distance >= 8.5f)
            {
                var newPosition = _playerTransform.position;
                newPosition.z = transform.position.z;
                transform.position = newPosition;
                Camera.main.orthographicSize = 4;
                return;
            }

            // TODO: Smooth transition
            var newPosition1 = _elevatorTransform.position;
            newPosition1.z = transform.position.z;
            transform.position = newPosition1;
            Camera.main.orthographicSize = 6;
        }
    }
}