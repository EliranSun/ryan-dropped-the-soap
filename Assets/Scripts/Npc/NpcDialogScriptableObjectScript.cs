using System;
using Dialog;
using UnityEngine;

namespace Npc
{
    [Serializable]
    [
        CreateAssetMenu(
            fileName = "NpcDialogScriptableObjectScript",
            menuName = "Scriptable Objects/NpcDialogScriptableObjectScript"
        )
    ]
    public class NpcDialogScriptableObjectScript : ScriptableObject
    {
        public NarrationDialogLine lastSpokenLine;
        [SerializeField] private NarrationDialogLine[] lines;
        [SerializeField] private int dialogIndex;

        public NarrationDialogLine GetNextLine()
        {
            if (lastSpokenLine)
                return lastSpokenLine;

            var line = lines[dialogIndex];
            if (dialogIndex + 1 < lines.Length) dialogIndex++;
            return line;
        }
    }
}