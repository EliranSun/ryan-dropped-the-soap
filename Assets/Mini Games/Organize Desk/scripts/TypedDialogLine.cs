using System;
using Dialog;

namespace Mini_Games.Organize_Desk.scripts
{
    [Serializable]
    public class TypedDialogLine
    {
        public NarrationDialogLine dialogLine;
        public DialogLineType type;

        public TypedDialogLine(NarrationDialogLine line, DialogLineType lineType)
        {
            dialogLine = line;
            type = lineType;
        }
    }
}