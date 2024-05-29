using UnityEngine;

namespace Ryan.Scripts {
    public class DeathByConcrete : MonoBehaviour {
        private void OnCollisionEnter2D(Collision2D other) {
            if (other.gameObject.CompareTag("Death by concrete"))
                EventManager.Instance.Publish(GameEvents.Dead);
        }
    }
}