using Object.Scripts;
using UnityEngine;

namespace Elevator.scripts
{
    public class SwitchedDoor : ObserverSubject
    {
        [SerializeField] private GameObject initDoor;
        [SerializeField] private GameObject alternateDoor;
        [SerializeField] private GameEvents enterDoorEvent;
        [SerializeField] private ObjectNames doorName;

        public void OnNotify(GameEventData data)
        {
            if (data.Name == GameEvents.PlayerInteraction)
            {
                var interactedObjectName = (ObjectNames)data.Data;
                if (interactedObjectName == ObjectNames.None)
                    return;

                if (interactedObjectName == initDoor.GetComponent<ObjectName>().objectName)
                {
                    initDoor.SetActive(false);
                    alternateDoor.SetActive(true);
                }

                if (interactedObjectName == alternateDoor.GetComponent<ObjectName>().objectName)
                {
                    initDoor.SetActive(true);
                    alternateDoor.SetActive(false);

                    print("Switching door: " + enterDoorEvent);
                    Notify(enterDoorEvent);
                }
            }
        }
    }
}