using UnityEngine;

namespace Object.Scripts
{
    // [RequireComponent(typeof(Rigidbody2D))]
    // [RequireComponent(typeof(ConstantForce2D))]

    public class BoatController : MonoBehaviour
    {
        [SerializeField] public float speed = 0.3f;

        // private ConstantForce2D _force;
        private bool _isMoving;

        private void Start()
        {
            // _force = GetComponent<ConstantForce2D>();
            // _force.enabled = false;
        }

        private void Update()
        {
            if (_isMoving)
                transform.Translate(new Vector3(1, 0, 0) * (speed * Time.deltaTime));
        }

        public void OnNotify(GameEventData gameEvent)
        {
            if (gameEvent.Name == GameEvents.BoatStart)
                // _force.enabled = true;
                _isMoving = true;
        }
    }
}