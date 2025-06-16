using Dialog;
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

            if (gameEvent.Name == GameEvents.LineNarrationEnd)
            {
                var prop = gameEvent.Data.GetType().GetProperty("_currentDialogue");
                if (prop == null)
                    return;

                var lastSpokenLine = (NarrationDialogLine)prop.GetValue(gameEvent.Data);

                dialog.lastSpokenLine = lastSpokenLine;
            }
        }
    }
}