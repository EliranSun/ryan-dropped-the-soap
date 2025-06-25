using System;
using System.Linq;
using Dialog;
using Elevator.scripts;
using UnityEngine;

namespace Npc
{
    public class NpcController : ObserverSubject
    {
        private const bool IsInApartment = true;
        [SerializeField] private BuildingController buildingController;
        [SerializeField] private Tenant tenant;
        [SerializeField] private NarrationDialogLine[] knockOnDoorLines;
        [SerializeField] private bool shouldOpenDoor = true;
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
                var actorNameProperty = eventData.Data.GetType().GetProperty("actorName");
                if (actorNameProperty == null) return;

                var actorName = (ActorName)actorNameProperty.GetValue(eventData.Data);
                if (IsActorMatchingTenant(actorName))
                    gameObject.SetActive(false);
            }

            if (eventData.Name == GameEvents.OpenNpcDoor)
            {
                var actionData = eventData.Data.GetType().GetProperty("actionNumberData");
                if (actionData == null) return;

                var doorNumber = (int)actionData.GetValue(eventData.Data);
                if (IsEligibleForDoorOpen(doorNumber)) Invoke(nameof(NpcOpenDoor), 1f);
            }
        }

        private bool IsActorMatchingTenant(ActorName actorName)
        {
            return string.Equals(actorName.ToString(), tenant.ToString(), StringComparison.CurrentCultureIgnoreCase);
        }

        private bool IsEligibleForDoorOpen(int doorNumber)
        {
            if (!gameObject.activeSelf)
                return false;

            var npcDoorNumber = _apartment.floorNumber * 10 + _apartment.apartmentNumber;

            print($"I am {tenant}. door knock on: {doorNumber}; my door:{npcDoorNumber}");

            if (npcDoorNumber != doorNumber) return false;
            if (!IsInApartment || knockOnDoorLines.Length == 0) return false;

            return true;
        }

        private void HandleDoorKnock(int doorNumber)
        {
            if (IsEligibleForDoorOpen(doorNumber))
            {
                Notify(GameEvents.TriggerSpecificDialogLine, knockOnDoorLines[0]);
                if (shouldOpenDoor) Invoke(nameof(NpcOpenDoor), 8f);
            }
        }

        private void NpcOpenDoor()
        {
            if (!_apartment.door) return;
            var doorController = _apartment.door.GetComponent<DoorController>();

            doorController.OpenNpcDoor();

            if (knockOnDoorLines.Length > 1)
                Notify(GameEvents.TriggerSpecificDialogLine, knockOnDoorLines[1]);
        }
    }
}