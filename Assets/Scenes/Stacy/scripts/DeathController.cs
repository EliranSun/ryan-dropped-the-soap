using UnityEngine;

namespace Scenes.Stacy.scripts
{
    public class DeathController : MonoBehaviour
    {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private PhysicsMaterial2D bodyMaterial;
        private bool _isDead;
        private bool _isHoldingBody;

        private void Update()
        {
            if (!_isDead || !_isHoldingBody) return;

            transform.position =
                new Vector3(playerTransform.position.x + 1, playerTransform.position.y - 1f, 0);
        }

        private void OnMouseDown()
        {
            if (!_isDead) return;

            var distanceFromPlayer = Mathf.Abs(Vector2.Distance(transform.position, playerTransform.position));

            if (distanceFromPlayer <= 2)
            {
                gameObject.layer = LayerMask.NameToLayer("NPC");
                gameObject.tag = "NPC";
                _isHoldingBody = true;
            }
        }

        private void OnMouseUp()
        {
            if (!_isDead) return;

            _isHoldingBody = false;
            gameObject.layer = LayerMask.NameToLayer("Ground");
            gameObject.tag = "Ground";
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.name == "knife")
            {
                _isDead = true;
                gameObject.layer = LayerMask.NameToLayer("Ground");
                gameObject.tag = "Ground";
                gameObject.GetComponent<Collider2D>().sharedMaterial = bodyMaterial;
                gameObject.GetComponent<Rigidbody2D>().mass *= 2;
            }
        }
    }
}