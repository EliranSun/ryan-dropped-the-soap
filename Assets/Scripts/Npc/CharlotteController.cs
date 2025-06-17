using System;
using Dialog;
using UnityEngine;

namespace Npc
{
    public class CharlotteController : ObserverSubject
    {
        [SerializeField] private GameObject playerBox;
        [SerializeField] private NarrationDialogLine[] lines;

        private void OnEnable()
        {
            Notify(GameEvents.TriggerSpecificDialogLine, GetNextLine());
        }

        private NarrationDialogLine GetNextLine()
        {
            var storedLastLineKey = PlayerPrefs.GetString("CharlotteLastSpokenLineKey");
            print("Stored key: " + storedLastLineKey + ";");

            if (storedLastLineKey == null) return lines[0];

            var lastLineIndex = -1;

            foreach (var line in lines)
                if (line.name == storedLastLineKey)
                    lastLineIndex = Array.IndexOf(lines, line);

            if (lastLineIndex == -1 || lastLineIndex >= lines.Length - 1) return lines[0];

            return lines[lastLineIndex];
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

                print("Store key: " + lastSpokenLine.name);
                PlayerPrefs.SetString("CharlotteLastSpokenLineKey", lastSpokenLine.name);
            }
        }
    }
}