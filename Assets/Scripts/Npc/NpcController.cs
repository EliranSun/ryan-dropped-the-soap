using Elevator.scripts;
using UnityEngine;

namespace Npc
{
    public class NpcController : MonoBehaviour
    {
        [SerializeField] private GameObject apartmentDoor;
        [SerializeField] private FloorData floorData;

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.KnockOnNpcDoor)
            {
                var doorNumber = (int)eventData.Data;

                // if (floorData.zekeFloorNumber == doorNumber)
                // {
                //     print("KNOCK ON ZEKE DOOR");
                //     var doorController = apartmentDoor.GetComponent<DoorController>();
                //     if (doorController) doorController.OpenNpcDoor();
                // }
            }
        }
    }
}