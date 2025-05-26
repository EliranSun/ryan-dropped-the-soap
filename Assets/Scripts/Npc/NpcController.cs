using System.Linq;
using Dialog;
using Elevator.scripts;
using UnityEngine;

namespace Npc
{
    public class NpcController : ObserverSubject
    {
        // public GameObject apartmentDoor;
        // public FloorData floorData;
        [SerializeField] private BuildingController buildingController;
        [SerializeField] private Tenant tenant;
        [SerializeField] private NarrationDialogLine[] knockOnDoorLines;
        private readonly bool _isInApartment = true;
        private TenantApartment _apartment;

        private void Start()
        {
            // FIXME:
            _apartment = buildingController.tenants.FirstOrDefault(t => t.name == tenant);
            print(_apartment);
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.KnockOnNpcDoor)
            {
                var doorNumber = (int)eventData.Data;
                var npcDoorNumber = _apartment.floorNumber * 10 + _apartment.apartmentNumber;
                if (npcDoorNumber == doorNumber)
                    if (_isInApartment && knockOnDoorLines.Length > 0)
                    {
                        Notify(GameEvents.TriggerSpecificDialogLine,
                            knockOnDoorLines[Random.Range(0, knockOnDoorLines.Length)]);

                        if (_apartment.door)
                            transform.position = _apartment.door.transform.position;
                    }
            }
        }
    }
}