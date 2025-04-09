using UnityEngine;
using UnityEngine.U2D;

// using UnityEngine.Rendering.Universal;

namespace Scenes.Stacy.scripts
{
    [RequireComponent(typeof(Light2DBase))]
    public class LightController : MonoBehaviour
    {
        [SerializeField] private Light2DBase light;

        public void OnNotify(GameEventData eventData)
        {
            // FIXME
            if (eventData.Name == GameEvents.DimLight) light.GetComponent<Light>().intensity -= 0.1f;
            if (eventData.Name == GameEvents.BrightenLight) light.GetComponent<Light>().intensity += 0.1f;
        }
    }
}