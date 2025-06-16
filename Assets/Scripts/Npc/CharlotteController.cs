using UnityEngine;

namespace Npc
{
    public class CharlotteController : ObserverSubject
    {
        [SerializeField] private NpcDialogScriptableObjectScript dialog;
        [SerializeField] private GameObject playerBox;

        private void OnEnable()
        {
            Notify(GameEvents.TriggerSpecificDialogLine, dialog.GetNextLine());
        }

        public void OnNotify(GameEventData gameEvent)
        {
            if (gameEvent.Name == GameEvents.FreePlayerFromBox)
                playerBox.SetActive(false);
        }
    }
}