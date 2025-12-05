using System;
using Dialog;
using Elevator.scripts;
using npc;
using UnityEngine;
using UnityEngine.Serialization;

namespace Npc
{
    [RequireComponent(typeof(NpcScriptableMovement))]
    public class NpcController : ObserverSubject
    {
        private const bool IsInApartment = true;
        [SerializeField] private Tenant tenant;
        [SerializeField] private bool shouldOpenDoor = true;
        [SerializeField] private GameObject apartmentDoor;
        [SerializeField] private NarrationDialogLine[] knockOnDoorLines;

        [FormerlySerializedAs("_doorNumber")] [SerializeField]
        private int tenantDoorNumber;

        private NpcScriptableMovement _scriptableMovement;

        private void Start()
        {
            _scriptableMovement = GetComponent<NpcScriptableMovement>();
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

            // if (eventData.Name == GameEvents.OpenNpcDoor)
            // var actionData = eventData.Data.GetType().GetProperty("actionNumberData");
            // if (actionData == null) return;
            //
            // var doorNumber = (int)actionData.GetValue(eventData.Data);
            // if (IsEligibleForDoorOpen(doorNumber)) Invoke(nameof(NpcOpenDoor), 1f);
        }

        private bool IsActorMatchingTenant(ActorName actorName)
        {
            return string.Equals(actorName.ToString(), tenant.ToString(), StringComparison.CurrentCultureIgnoreCase);
        }

        private bool IsEligibleForDoorOpen(int doorNumber)
        {
            if (!gameObject.activeSelf)
                return false;

            if (tenantDoorNumber != doorNumber)
                return false;

            return IsInApartment && knockOnDoorLines.Length != 0;
        }

        private void HandleDoorKnock(int doorNumber)
        {
            if (IsEligibleForDoorOpen(doorNumber))
            {
                // var randomLine = knockOnDoorLines[Random.Range(0, knockOnDoorLines.Length)];
                // _scriptableMovement.ReplacePointOfInterest(apartmentDoor.transform);
                Notify(GameEvents.TriggerSpecificDialogLine, knockOnDoorLines[0]);

                if (shouldOpenDoor)
                    Invoke(nameof(NpcOpenDoor), 3f);
            }
        }

        private void NpcOpenDoor()
        {
            // if (!_apartment.door) return;
            // var doorController = _apartment.door.GetComponent<DoorController>();
            //
            // doorController.OpenDoor();

            Notify(GameEvents.OpenNpcDoor, tenantDoorNumber);


            if (knockOnDoorLines.Length > 1)
                Notify(GameEvents.TriggerSpecificDialogLine, knockOnDoorLines[1]);
        }
    }
}