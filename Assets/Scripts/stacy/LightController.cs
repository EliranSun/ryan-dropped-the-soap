using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Scenes.Stacy.scripts
{
    [RequireComponent(typeof(Light2D))]
    public class LightController : MonoBehaviour
    {
        [SerializeField] private Light2D lightSource;

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.DimLight) lightSource.intensity -= 0.1f;
            if (eventData.Name == GameEvents.BrightenLight) lightSource.intensity += 0.1f;
        }
    }
}