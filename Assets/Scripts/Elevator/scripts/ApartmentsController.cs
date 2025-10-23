using System;
using Dialog;
using Object.Scripts;
using Player;
using UnityEngine;

namespace Elevator.scripts
{
    [Serializable]
    public class DoorInfo
    {
        public ActorName residentName;
        public GameObject door;
    }

    public class ApartmentsController : ObserverSubject
    {
        [SerializeField] private FloorData floorData;
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

                    if (doorNumber == floorData.CharlotteApartmentNumber)
                    {
                        print("Knock on Charlotte");
                        Notify(GameEvents.KnockOnNpcDoor, new DoorInfo
                        {
                            residentName = ActorName.Charlotte,
                            door = door.gameObject
                        });
                    }
                    else
                    {
                        print($"Knock on vacant apartment {doorNumber}");
                    }
                }
            }
        }
    }
}