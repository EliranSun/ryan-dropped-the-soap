using UnityEngine;

namespace Npc
{
    public class CharlotteController : ObserverSubject
    {
        [SerializeField] private NpcDialogScriptableObjectScript dialog;

        private void OnEnable()
        {
            Notify(GameEvents.TriggerSpecificDialogLine, dialog.GetNextLine());
        }

        public void OnNotify(GameEventData gameEvent)
        {
            if (gameEvent.Name == GameEvents.PlayerApartmentDoorOpened)
            {
            }
            // if (npcKnockingOnPlayerApartment)
            // {
            //     StopAllCoroutines();
            //     npcKnockingOnPlayerApartment.transform.position = playerApartmentHallwayDoor.transform.position;
            //     npcKnockingOnPlayerApartment = null;
            //
            //     if (initLine) Notify(GameEvents.TriggerSpecificDialogLine, initLine);
            // }
            //
            // break;
        }
    }
}