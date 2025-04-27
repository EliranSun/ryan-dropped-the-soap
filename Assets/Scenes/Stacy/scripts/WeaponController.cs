using UnityEngine;

namespace Scenes.Stacy.scripts
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer playerSpriteRenderer;
        private GameObject _weaponStuckIn;

        private void Update()
        {
            if (_weaponStuckIn)
            {
                var newPosition = _weaponStuckIn.transform.position;
                newPosition.x = transform.position.x + (playerSpriteRenderer.flipX ? 0.5f : -0.5f);
                newPosition.y = transform.position.y;

                _weaponStuckIn.transform.position = newPosition;
            }

            if (Input.GetButtonDown("Fire1")) ForcePushVictim();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("NPC"))
                _weaponStuckIn = collision.gameObject;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("NPC"))
                _weaponStuckIn = null;
        }

        private void ForcePushVictim()
        {
            if (_weaponStuckIn)
            {
                var xForce = 400 * (playerSpriteRenderer.flipX ? 1 : -1);
                // unlock z axis rotation
                _weaponStuckIn.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                _weaponStuckIn.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector3(xForce, 0, 1000));
                _weaponStuckIn = null;
            }
        }
    }
}