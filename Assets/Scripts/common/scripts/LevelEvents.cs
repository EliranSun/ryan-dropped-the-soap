using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace common.scripts
{
    public class LevelEvents : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Light2D globalLight;

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.name == GameEvents.TriggerAlarmClockStop)
            {
                mainCamera.gameObject.AddComponent<Zoom>();
                mainCamera.gameObject.GetComponent<Zoom>().endSize = 8;
                globalLight.intensity = 1f;
            }
        }
    }
}