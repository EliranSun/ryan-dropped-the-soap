using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Scenes.Stacy.scripts
{
    [RequireComponent(typeof(Light2D))]
    public class LightController : MonoBehaviour
    {
        [SerializeField] private Light2D light;

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.DimLight) light.intensity -= 0.1f;
            if (eventData.Name == GameEvents.BrightenLight) light.intensity += 0.1f;
        }
    }
}