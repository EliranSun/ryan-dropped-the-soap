using System.Linq;
using Elevator.scripts;
using UnityEngine;

namespace Npc
{
    public class NpcController : MonoBehaviour
    {
        public GameObject apartmentDoor;
        public FloorData floorData;
        [SerializeField] private BuildingController buildingController;
        [SerializeField] private Tenant tenant;
        private TenantApartment apartment;

        private void Start()
        {
            // FIXME:
            apartment = buildingController.tenants.FirstOrDefault(t => t.name == tenant);
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.KnockOnNpcDoor)
            {
                var doorNumber = (int)eventData.Data;
                // if (buildingController.tenants == doorNumber)
                {
                    // print("KNOCK ON ZEKE DOOR");
                    // var doorController = apartmentDoor.GetComponent<DoorController>();
                    // if (doorController) doorController.OpenNpcDoor();
                }
            }
        }
    }
}