using UnityEngine;

namespace common.scripts
{
    public class LevelEvents : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.name == GameEvents.TriggerAlarmClockStop)
            {
                mainCamera.gameObject.AddComponent<Zoom>();
                mainCamera.gameObject.GetComponent<Zoom>().endSize = 8;
            }
        }
    }
}