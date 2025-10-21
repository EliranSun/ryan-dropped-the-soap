using Object.Scripts;
using Player;
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
                var interactedObject = (Interaction)data.Data;
                if (interactedObject.objectName == ObjectNames.None)
                    return;

                if (interactedObject.objectName == initDoor.GetComponent<ObjectName>().objectName)
                {
                    initDoor.SetActive(false);
                    alternateDoor.SetActive(true);
                }

                if (interactedObject.objectName == alternateDoor.GetComponent<ObjectName>().objectName)
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