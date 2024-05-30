using UnityEngine;

namespace Ryan.Scripts {
    [RequireComponent(typeof(Collider2D))]
    public class DeathByConcrete : MonoBehaviour {
        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.CompareTag("Death by concrete"))
                EventManager.Instance.Publish(GameEvents.Dead);
        }
    }
}