using UnityEngine;

namespace Object.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(ConstantForce2D))]
    public class BoatController : MonoBehaviour
    {
        private ConstantForce2D _force;

        private void Start()
        {
            _force = GetComponent<ConstantForce2D>();
            _force.enabled = false;
        }

        public void OnNotify(GameEventData gameEvent)
        {
            if (gameEvent.name == GameEvents.BoatStart)
                _force.enabled = true;
        }
    }
}