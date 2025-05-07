using System;
using Dialog;
using UnityEngine;

namespace Character_Creator.scripts
{
    [Serializable]
    public class DialogLineEvents
    {
        public GameEvents eventName;
        public NarrationDialogLine dialogLine;
    }

    public class EventToDialogMap : MonoBehaviour
    {
        [SerializeField] public DialogLineEvents[] dialogLineEvents;
    }
}