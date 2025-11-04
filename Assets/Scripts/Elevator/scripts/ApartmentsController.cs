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
            if (data.Name is GameEvents.UnlockRyanApartment or GameEvents.UnlockCharlotteApartment)
            {
                var door = FindNpcDoor(floorData.CharlotteApartmentNumber);
                door.SetActive(false);
            }

            if (data.Name == GameEvents.PlayerInteraction)
            {
                var interactedObject = (Interaction)data.Data;
                if (interactedObject.objectName == ObjectNames.ApartmentDoor)
                {
                    var door = FindNpcDoor(floorData.CharlotteApartmentNumber);
                    Notify(GameEvents.KnockOnNpcDoor, new DoorInfo
                    {
                        residentName = ActorName.Charlotte,
                        door = door.gameObject
                    });
                }
            }
        }

        private GameObject FindNpcDoor(int npcDoorNumber)
        {
            // Find the door whose instanceID matches doorId
            var door = Array.Find(doors, d =>
                d.gameObject.GetComponent<DoorController>().doorNumber == npcDoorNumber
            );

            if (door.doorNumber == npcDoorNumber) return door.gameObject;

            print($"Knock on vacant apartment {door.doorNumber}");
            return null;
        }
    }
}