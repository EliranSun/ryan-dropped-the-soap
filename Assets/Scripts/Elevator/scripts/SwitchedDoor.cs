using UnityEngine;

namespace Elevator.scripts
{
    public class SwitchedDoor : MonoBehaviour
    {
        [SerializeField] private GameObject initDoor;
        [SerializeField] private GameObject alternateDoor;

        public void OnNotify(GameEventData data)
        {
            if (data.Name == GameEvents.PlayerInteraction)
            {
                var interactedObject = (GameObject)data.Data;
                if (!interactedObject || !interactedObject.GetComponent<ObjectName>())
                    return;

                var interactedObjectName = interactedObject.GetComponent<ObjectName>().objectName;

                if (interactedObjectName == initDoor.GetComponent<ObjectName>().objectName)
                {
                    initDoor.SetActive(false);
                    alternateDoor.SetActive(true);
                }

                if (interactedObjectName == alternateDoor.GetComponent<ObjectName>().objectName)
                {
                    initDoor.SetActive(true);
                    alternateDoor.SetActive(false);
                }
            }
        }
    }
}