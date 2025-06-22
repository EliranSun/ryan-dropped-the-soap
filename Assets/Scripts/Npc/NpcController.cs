using System;
using System.Linq;
using Dialog;
using Elevator.scripts;
using UnityEngine;

namespace Npc
{
    public class NpcController : ObserverSubject
    {
        [SerializeField] private BuildingController buildingController;
        [SerializeField] private Tenant tenant;
        [SerializeField] private NarrationDialogLine[] knockOnDoorLines;
        private readonly bool _isInApartment = true;
        private TenantApartment _apartment;

        private void Start()
        {
            _apartment = buildingController.tenants.FirstOrDefault(t => t.name == tenant);
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.KnockOnNpcDoor)
            {
                var doorNumber = (int)eventData.Data;
                HandleDoorKnock(doorNumber);
            }

            if (eventData.Name == GameEvents.HideSelf)
            {
                var actorName = (ActorName)eventData.Data;
                if (string.Equals(actorName.ToString(), tenant.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    gameObject.SetActive(false);
            }
        }

        private void HandleDoorKnock(int doorNumber)
        {
            if (!gameObject.activeSelf)
                return;

            var npcDoorNumber = _apartment.floorNumber * 10 + _apartment.apartmentNumber;

            print($"I am {tenant}. door knock on: {doorNumber}; my door:{npcDoorNumber}");

            if (npcDoorNumber != doorNumber) return;
            if (!_isInApartment || knockOnDoorLines.Length == 0) return;

            Notify(GameEvents.TriggerSpecificDialogLine, knockOnDoorLines[0]);
            Invoke(nameof(NpcOpenDoor), 8f);
        }

        private void NpcOpenDoor()
        {
            if (!_apartment.door) return;
            var doorController = _apartment.door.GetComponent<DoorController>();

            doorController.OpenNpcDoor();
            Notify(GameEvents.TriggerSpecificDialogLine, knockOnDoorLines[1]);
        }
    }
}