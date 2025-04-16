using System.Collections;
using System.Collections.Generic;
using Dialog.Scripts;
using UnityEngine;

public class TriggerDialogOnObjectClick : ObserverSubject
{
    [SerializeField] NarrationDialogLine[] lines;
    private int _clickCount;

    private void OnMouseDown()
    {
        if (_clickCount >= lines.Length) return;

        Notify(GameEvents.TriggerSpecificDialogLine, lines[_clickCount]);
        _clickCount++;
    }
}
