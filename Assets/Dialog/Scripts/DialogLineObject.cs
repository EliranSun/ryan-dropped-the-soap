using UnityEngine;

namespace Dialog.Scripts {
    [CreateAssetMenu(fileName = "Line", menuName = "Dialogue/Line")]
    public class DialogLineObject : ScriptableObject {
        public string subtitles;
        public AudioClip line;
        public DialogLineObject nextDialogueLine;
        public float waitAfterLine;
    }
}