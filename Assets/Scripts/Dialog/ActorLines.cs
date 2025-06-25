using UnityEngine;

namespace Dialog
{
    public class ActorLines : ObserverSubject
    {
        [SerializeField] private NarrationDialogLine[] lines;

        private void Start()
        {
            if (lines.Length > 0) Invoke(nameof(InvokeLine), 2);
        }

        private void InvokeLine()
        {
            Notify(GameEvents.TriggerSpecificDialogLine, lines[0]);
        }
    }
}