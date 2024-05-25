using System;
using UnityEngine;

namespace Dialog.Scripts {
    [Serializable]
    public enum DialogAction {
        None,
        NextLevel,
        RestartLevel,
        ZoomOut
    }

    [CreateAssetMenu(fileName = "Line", menuName = "Dialogue/Line")]
    public class DialogLineObject : ScriptableObject {
        public string subtitles;
        public AudioClip line;
        public DialogLineObject nextDialogueLine;
        public float waitAfterLine;
        public DialogAction afterLineAction;
        public bool isMuffled = true;
    }
}