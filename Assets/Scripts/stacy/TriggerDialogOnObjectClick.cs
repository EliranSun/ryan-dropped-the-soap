using Dialog;
using UnityEngine;

namespace Scenes.Stacy.scripts
{
    public class TriggerDialogOnObjectClick : ObserverSubject
    {
        [SerializeField] private NarrationDialogLine[] lines;
        private int _clickCount;

        private void OnMouseDown()
        {
            if (_clickCount >= lines.Length) return;

            var line = lines[_clickCount];

            if (line.lineCondition.condition != Condition.None && !line.lineCondition.isMet)
                return;

            Notify(GameEvents.TriggerSpecificDialogLine, line);
            _clickCount++;
        }
    }
}