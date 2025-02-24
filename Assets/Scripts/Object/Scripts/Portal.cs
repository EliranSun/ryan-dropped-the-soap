using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace Object.Scripts
{
    [RequireComponent(typeof(Collider2D))]
    public class Portal : MonoBehaviour
    {
        [SerializeField] private GameObject[] objectsToPortalize;
        [SerializeField] private Transform portalExitPoint;
        [SerializeField] private bool affectXOnly = true;

        private void OnTriggerEnter2D(Collider2D other)
        {
            print($"OnTriggerEnter2D: {other.gameObject.name}");

            foreach (var obj in objectsToPortalize)
            {
                if (obj == other.gameObject)
                {
                    // teleport the object to the portal exit point
                    if (affectXOnly)
                    {
                        other.transform.position = new Vector3(portalExitPoint.position.x, other.transform.position.y, other.transform.position.z);
                    }
                    else
                    {
                        other.transform.position = portalExitPoint.position;
                    }

                    break;
                }
            }
        }
    }
}