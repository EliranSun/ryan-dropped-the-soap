using UnityEngine;
using UnityEngine.U2D;

namespace Elevator.scripts
{
    public class ShaftLightController : MonoBehaviour
    {
        [SerializeField] private GameObject shaftFloorLight;

        private void Start()
        {
            shaftFloorLight.GetComponent<Light2DBase>().enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Light Trigger")) shaftFloorLight.GetComponent<Light2DBase>().enabled = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Light Trigger")) shaftFloorLight.GetComponent<Light2DBase>().enabled = false;
        }
    }
}