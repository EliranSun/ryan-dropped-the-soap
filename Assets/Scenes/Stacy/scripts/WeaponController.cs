using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer playerSpriteRenderer;
    private GameObject weaponStuckIn;

    private void Update()
    {
        if (weaponStuckIn)
        {
            var newPosition = weaponStuckIn.transform.position;
            newPosition.x = transform.position.x + (playerSpriteRenderer.flipX ? 0.5f : -0.5f);
            newPosition.y = transform.position.y;

            weaponStuckIn.transform.position = newPosition;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("NPC"))
        {
            weaponStuckIn = collision.gameObject;
            Invoke(nameof(ForcePushVictim), 1f);
        }
    }

    private void ForcePushVictim()
    {
        if (weaponStuckIn)
        {
            // unlock z axis rotation
            weaponStuckIn.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            var xForce = 500 * (playerSpriteRenderer.flipX ? 1 : -1);
            weaponStuckIn.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector3(xForce, 0,  100));
            weaponStuckIn = null;
        }
    }
}
