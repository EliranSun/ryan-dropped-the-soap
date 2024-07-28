using UnityEngine;

namespace Object.Scripts
{
    public class CoinController : MonoBehaviour
    {
        private void Start()
        {
            Destroy(gameObject, 5);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Death by concrete"))
                Destroy(gameObject);
        }
    }
}