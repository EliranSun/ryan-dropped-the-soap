using UnityEngine;
using UnityEngine.UI;

namespace Elevator.scripts
{
    public class FloorsToPanelButtons : MonoBehaviour
    {
        public void OnNotify(GameEventData eventData)
        {
            if (eventData.name == GameEvents.FloorsUpdate)
            {
                var floors = (int)eventData.data;
                var panel = GameObject.Find("Panel");
                var panelChildren = panel.transform.childCount;
                for (var i = 0; i < panelChildren; i++)
                {
                    var child = panel.transform.GetChild(i);
                    var button = child.GetComponent<Button>();
                }
            }
        }
    }
}