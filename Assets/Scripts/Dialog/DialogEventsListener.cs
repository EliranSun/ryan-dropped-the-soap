using System;
using System.Linq;
using UnityEngine;

// old
namespace Dialog.Scripts
{
    [Serializable]
    public class EventLineObject
    {
        [SerializeField] public GameEvents name;
        [SerializeField] public DialogLineObject line;
    }

    public class DialogEventsListener : MonoBehaviour
    {
        [SerializeField] private EventLineObject[] events;

        public void OnNotify(GameEventData eventData)
        {
            try
            {
                var foundEvent = events.First(item => item.name == eventData.Name);
                if (foundEvent.line == null)
                    return;

                DialogSystem.Instance.TriggerLine(foundEvent.line);
            }
            catch (InvalidOperationException)
            {
                // print($"Event {eventData.name} does not have any lines");
            }
        }
    }
}