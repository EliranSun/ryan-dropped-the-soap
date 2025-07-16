using System;
using System.Collections;
using Dialog;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Npc
{
    public class CharlotteController : ObserverSubject
    {
        [SerializeField] private bool isAtPlayerApartment;
        [SerializeField] private bool isKnockingOnPlayerDoor;

        [SerializeField] private NarrationDialogLine[] lines;
        [SerializeField] private NarrationDialogLine knockingLine;
        [SerializeField] private NarrationDialogLine theoryLine;
        [SerializeField] private NarrationDialogLine playerGrewLine;
        [SerializeField] private NarrationDialogLine goToRooftopLine;

        [SerializeField] private GameObject playerApartmentDoor;
        private bool _awaitGoingRooftop;
        private bool _awaitsPlayerGrowthTheory;
        private bool _isExitingDoor;
        private int _playerGrowthLevel;

        private void Start()
        {
            _awaitGoingRooftop = PlayerPrefs.GetInt("CharlotteAwaitGoingRooftop", 0) == 1;

            if (isAtPlayerApartment)
                Invoke(nameof(KnockOnDoor), 1);

            if (_awaitGoingRooftop && SceneManager.GetActiveScene().name == "inside elevator")
                StartCoroutine(TriggerLine(goToRooftopLine));
        }

        private void OnEnable()
        {
            // Notify(GameEvents.TriggerSpecificDialogLine, GetNextLine());
            // gameObject.SetActive(isAtPlayerApartment);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Apartment Door") && _isExitingDoor)
                Invoke(nameof(DisableSelf), 0.5f);
        }

        private void DisableSelf()
        {
            gameObject.SetActive(false);
        }

        private void ExitPlayerApartment()
        {
            _isExitingDoor = true;
            _awaitGoingRooftop = true;
            PlayerPrefs.SetInt("CharlotteAwaitGoingRooftop", 1);
            Notify(GameEvents.NpcGoTo, playerApartmentDoor);
        }

        private void KnockOnDoor()
        {
            if (isKnockingOnPlayerDoor)
                Notify(GameEvents.TriggerSpecificDialogLine, knockingLine);
        }

        private IEnumerator TriggerLine(NarrationDialogLine line)
        {
            yield return new WaitForSeconds(1);
            Notify(GameEvents.TriggerSpecificDialogLine, line);
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

                case GameEvents.ExitPlayerApartment:
                {
                    var actorNameProperty = gameEvent.Data.GetType().GetProperty("actorName");
                    if (actorNameProperty == null)
                        break;

                    var actorName = (ActorName)actorNameProperty.GetValue(gameEvent.Data);

                    if (actorName == ActorName.Charlotte)
                        Invoke(nameof(ExitPlayerApartment), 1);
                    break;
                }

                case GameEvents.PlayerGrew:
                {
                    var levelProperty = gameEvent.Data.GetType().GetProperty("level");
                    if (levelProperty != null)
                    {
                        _playerGrowthLevel = (int)levelProperty.GetValue(gameEvent.Data);
                        if (_playerGrowthLevel == 4)
                            StartCoroutine(TriggerLine(playerGrewLine));
                    }

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
}