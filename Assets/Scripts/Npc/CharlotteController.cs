using System;
using Dialog;
using UnityEngine;

namespace Npc
{
    public class CharlotteController : ObserverSubject
    {
        [SerializeField] private GameObject playerBox;
        [SerializeField] private bool isAtPlayerApartment;
        [SerializeField] private bool isKnockingOnPlayerDoor;
        [SerializeField] private NarrationDialogLine[] lines;
        [SerializeField] private NarrationDialogLine knockingLine;
        [SerializeField] private NarrationDialogLine playerIsFreeLine;
        [SerializeField] private NarrationDialogLine theoryLine;
        private bool _awaitsPlayerGrowthTheory;

        private void OnEnable()
        {
            // Notify(GameEvents.TriggerSpecificDialogLine, GetNextLine());
            // gameObject.SetActive(isAtPlayerApartment);
        }

        private void Start()
        {
            if (isAtPlayerApartment) Invoke(nameof(KnockOnDoor), 1);
        }

        private void KnockOnDoor()
        {
            if (isKnockingOnPlayerDoor) 
                Notify(GameEvents.TriggerSpecificDialogLine, knockingLine);
        }

        private NarrationDialogLine GetNextLine()
        {
            var storedLastLineKey = PlayerPrefs.GetString("CharlotteLastSpokenLineKey");
            if (storedLastLineKey == null) return lines[0];

            var lastLineIndex = FindLineIndexByName(storedLastLineKey);
            if (lastLineIndex == -1 || lastLineIndex > lines.Length - 1) return lines[0];

            return lines[lastLineIndex];
        }

        private int FindLineIndexByName(string lineName)
        {
            var lineIndex = -1;
            foreach (var line in lines)
                if (line.name == lineName)
                    lineIndex = Array.IndexOf(lines, line);

            return lineIndex;
        }

        public void OnNotify(GameEventData gameEvent)
        {
            switch (gameEvent.Name)
            {
                // case GameEvents.FreePlayerFromBox:
                //     playerBox.SetActive(false);
                //     break;

                case GameEvents.LineNarrationEnd:
                {
                    var prop = gameEvent.Data.GetType().GetProperty("_currentDialogue");
                    if (prop == null)
                        return;

                    var lastSpokenLine = (NarrationDialogLine)prop.GetValue(gameEvent.Data);
                    var lineIndex = FindLineIndexByName(lastSpokenLine.name);

                    if (lineIndex != -1) // this will ensure we replay the correct line on scene restart
                        PlayerPrefs.SetString("CharlotteLastSpokenLineKey", lastSpokenLine.name);

                    break;
                }

                case GameEvents.CharlotteWaitingTheory:
                    _awaitsPlayerGrowthTheory = true;
                    break;

                case GameEvents.PlayerGrew:
                    if (_awaitsPlayerGrowthTheory)
                    {
                        _awaitsPlayerGrowthTheory = false;
                        Notify(GameEvents.TriggerSpecificDialogLine, theoryLine);
                    }

                    break;
            }
        }
    }
}