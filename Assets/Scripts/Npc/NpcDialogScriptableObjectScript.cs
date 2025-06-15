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
        [SerializeField] private NarrationDialogLine[] lines;
        [SerializeField] private int dialogIndex;

        public NarrationDialogLine GetNextLine()
        {
            var line = lines[dialogIndex];
            if (dialogIndex + 1 < lines.Length) dialogIndex++;
            return line;
        }
    }
}