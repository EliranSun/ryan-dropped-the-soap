using System;
using System.Linq;
using UnityEngine;

namespace Dialog.Scripts {
    [Serializable]
    public class EventLineObject {
        [SerializeField] public GameEvents name;
        [SerializeField] public DialogLineObject line;
    }

    public class DialogEventsListener : MonoBehaviour {
        [SerializeField] private EventLineObject[] events;

        public void OnNotify(GameEventData eventData) {
            print($"DialogueEventsListener: Notified on {eventData.name}");

            try {
                var foundEvent = events.First(item => item.name == eventData.name);
                if (foundEvent.line == null)
                    return;

                DialogSystem.Instance.TriggerLine(foundEvent.line);
            }
            catch (InvalidOperationException) {
                print($"Event {eventData.name} not found");
            }
        }
    }
}