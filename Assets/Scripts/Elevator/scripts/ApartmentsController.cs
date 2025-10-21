using System;
using Object.Scripts;
using Player;
using UnityEngine;

namespace Elevator.scripts
{
    public class ApartmentsController : MonoBehaviour
    {
        [SerializeField] private DoorController[] doors;

        public void OnNotify(GameEventData data)
        {
            if (data.Name == GameEvents.PlayerInteraction)
            {
                var interactedObject = (Interaction)data.Data;
                if (interactedObject.objectName == ObjectNames.ApartmentDoor)
                {
                    var doorId = interactedObject.objectId;
                    // Find the door whose instanceID matches doorId
                    var door = Array.Find(doors, d => d.gameObject.GetInstanceID() == doorId);
                    var doorNumber = door ? door.doorNumber : -1;
                    print($"Knock on apartment {doorNumber}");
                }
            }
        }
    }
}