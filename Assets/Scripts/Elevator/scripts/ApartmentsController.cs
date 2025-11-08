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
                if (door) door.OpenDoor();
            }

            if (data.Name == GameEvents.PlayerInteraction)
            {
                var interactedObject = (Interaction)data.Data;
                var doorId = interactedObject.objectId;

                if (interactedObject.objectName == ObjectNames.ApartmentDoor)
                {
                    var door = CompareNpcDoor(doorId, floorData.CharlotteApartmentNumber);
                    if (door != null)
                        Notify(GameEvents.KnockOnNpcDoor, new DoorInfo
                        {
                            residentName = ActorName.Charlotte,
                            door = door.gameObject
                        });
                }
            }
        }

        private DoorController CompareNpcDoor(int doorId, int npcDoorNumber)
        {
            // Find the door whose instanceID matches doorId
            var door = Array.Find(doors, d =>
                d.gameObject.GetInstanceID() == doorId
            );

            if (door.doorNumber == npcDoorNumber)
            {
                print($"Match id {doorId} to NPC door number {door.doorNumber}");
                return door;
            }

            print($"Did not find NPC door for id {doorId}");
            return null;
        }

        public DoorController FindNpcDoor(int npcDoorNumber)
        {
            var door = Array.Find(doors, d =>
                d.gameObject.GetComponent<DoorController>().doorNumber == npcDoorNumber
            );

            if (door.doorNumber == npcDoorNumber) return door;

            print($"Knock on vacant apartment {door.doorNumber}");
            return null;
        }
    }
}