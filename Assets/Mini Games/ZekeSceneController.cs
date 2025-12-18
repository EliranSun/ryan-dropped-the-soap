using System;
using System.Collections.Generic;
using System.Linq;
using Dialog;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mini_Games
{
    /* objects vs. faces - work vs. personal - logic vs. motion -
     *
     * Sunday
     * 10:00 - organize
     * 12:00 - avoid chit chat (flirt)
     * 14:00 - lock pick - CONTROL YOURSELF
     *              maybe reverse lock pick, boss is showing a face amd
     *              zeke tries to understand what his boss feels, which connects
     *              to him shutting off his own feelings to get that promotion;
     *
     *              no and - control yourself. zeke manages his own feelings in the GOOD
     *              employee flow
     * 16:00 - organize
     * 18:00 - home - neglect wife
     * 20:00 - sleep early (snooze backward)
     * 22:00 - organize in your sleep
     *
     * Monday (speed up)
     * 8:00 - wake up on time (snooze)
     * ...
     *
     * Monday (speed up)
     * ...
     * 18:00 - talk with boss
     * 20:00 - wife not at home, commit suicide
     * TODO: This can be elevated by a series of choices:
     * - Zeke first sees a note from why stating: "I can't take this anymore" with her ring on the table
     * - He then is standing for a couple of seconds starting, then a series of choices for the player
     * - What should I do next? STARE | PUNCH THE WALL | BURN THE PAPER | CRY
     * - after choosing each one, the other choices remains. when there's nothing left,
     * - Cry: there were no tears left. There is nothing left for me
     * - Should I do it? YES | NO
     * - Choosing NO will reset the options, until the player chooses yes, then
     * - scene change to like john locke suicide, with shadow then Zeke's figure, hung
     *
     *
     *
     *
     * after the suicide,
     * Zeke: What the fuck what that...?
     * Player (NPC): What was what?
     * Zeke (Player): Who are you? | I just had... a dream? | No it was real. I just experienced... something.
     */

    [Serializable]
    public enum LocationName
    {
        OfficeOpenSpace,
        BossOffice,
        Home
    }

    [Serializable]
    public class MiniGameLocation
    {
        public MiniGameName name;
        public LocationName location;
    }

    [Serializable]
    public class Location
    {
        public Transform transform;
        public LocationName location;
    }

    public class ZekeSceneController : ObserverSubject
    {
        [SerializeField] private NarrationDialogLine sceneStartLine;
        [SerializeField] private NarrationDialogLine zekeFirstQuestion;
        [SerializeField] private NarrationDialogLine zekeLastQuestion;
        [SerializeField] private NarrationDialogLine morganContactCharlotte;
        [SerializeField] private GameObject theClock;
        [SerializeField] private GameObject charlotte;
        [SerializeField] private GameObject noteOnTable;
        [SerializeField] private GameObject suicideCutsceneWrapper;
        [SerializeField] private GameObject noteCutscene;
        [SerializeField] private GameObject player;
        [SerializeField] private int deadlineDays = 2;
        [SerializeField] private int deadlineHour = 18;
        [SerializeField] private float advanceIntervalInSeconds;
        [SerializeField] private MiniGameLocation[] miniGameLocations;
        [SerializeField] private Location[] locations;
        private readonly List<string> _choicesMade = new();
        private readonly string[] _dayNames = { "Sunday", "Monday", "Tuesday" };
        private Vector2 _currentMiniGamePosition;
        private int _currentTimeInMinutes = 9 * 60; // "Sunday, 8:00"
        private int _deadlineTime;
        private bool _suicideChoicesStarted;

        private void Start()
        {
            noteOnTable.SetActive(false);
            _deadlineTime = CalcDeadline();
            Notify(GameEvents.TriggerSpecificDialogLine, sceneStartLine);
        }

        private int CalcDeadline()
        {
            return deadlineDays * 24 * 60 + deadlineHour * 60;
        }

        public void OnNotify(GameEventData eventData)
        {
            switch (eventData.Name)
            {
                case GameEvents.NextScene:
                    Invoke(nameof(ChangeScene), 5f);
                    break;

                case GameEvents.PlayerChoice:
                {
                    if (!_suicideChoicesStarted) break;
                    var choice = (PlayerChoice)eventData.Data;
                    print($"CHOICES MADE {_choicesMade.Count}");

                    if (choice.actionAfterPlayerChoice == GameEvents.ResetSuicideChoices)
                        _choicesMade.RemoveAt(0);

                    if (choice.actionAfterPlayerChoice == GameEvents.TriggerZekeSuicide)
                    {
                        SetSuicideScene();
                        break;
                    }

                    if (_choicesMade.Count >= 3)
                        Invoke(nameof(TriggerLastQuestion), 0.5f);

                    if (choice.text != "...")
                        if (!_choicesMade.Contains(choice.text))
                            _choicesMade.Add(choice.text);

                    break;
                }

                case GameEvents.ZekeReadWifeNote:
                    noteCutscene.SetActive(true);
                    Invoke(nameof(SetSuicideQuestionsScene), 5);
                    break;

                case GameEvents.ZekeGoodEmployeeEnding:

                    break;

                case GameEvents.ZekePositionHome:
                    Invoke(nameof(FinalGoodEmployeeSequence), 2);
                    break;

                case GameEvents.MiniGameStart or GameEvents.StartMiniGames:
                {
                    // var isFirstTime = eventData.Name == GameEvents.StartMiniGames;
                    // if (_currentTimeInMinutes >= _deadlineTime)
                    // {
                    //     GoodEmployeeEnding();
                    //     break;
                    // }
                    //
                    // if (isFirstTime) return;
                    //
                    // var miniGameName = (MiniGameName)eventData.Data;
                    // var currentMiniGame = miniGameLocations.First(item => item.name == miniGameName);
                    // var location = locations.First(item => item.location == currentMiniGame.location);
                    //
                    // if (_currentMiniGamePosition == (Vector2)location.transform.position)
                    //     return;
                    //
                    // _currentMiniGamePosition = location.transform.position;
                    // player.transform.position = _currentMiniGamePosition;
                    break;
                }
            }
        }

        private void GoodEmployeeEnding()
        {
            Notify(GameEvents.EndMiniGames);
            _currentTimeInMinutes = _deadlineTime;

            PositionPlayerAt(LocationName.BossOffice);

            charlotte.SetActive(false);
            noteOnTable.SetActive(true);
        }

        private void ChangeScene()
        {
            SceneManager.LoadScene("Scenes/Post zeke suicide");
        }

        private void TriggerMorganContactCharlotte()
        {
            Notify(GameEvents.TriggerSpecificDialogLine, morganContactCharlotte);
        }

        private void TriggerLastQuestion()
        {
            Notify(GameEvents.TriggerSpecificDialogLine, zekeLastQuestion);
        }

        private void SetSuicideQuestionsScene()
        {
            _suicideChoicesStarted = true;

            noteCutscene.SetActive(false);
            noteOnTable.SetActive(false);

            // PositionPlayerAt(LocationName.Home);

            Notify(GameEvents.StopMusic);
            Notify(GameEvents.TriggerSpecificDialogLine, zekeFirstQuestion);

            PositionClockAtCenter();
        }

        private void SetSuicideScene()
        {
            _suicideChoicesStarted = false;
            suicideCutsceneWrapper.SetActive(true);
            TriggerMorganContactCharlotte();
        }

        private void PositionClockAtCenter()
        {
            // Calculate the center of the screen in world coordinates and set theClock to that position
            var canvas = theClock.GetComponentInParent<Canvas>();
            if (canvas != null && canvas.renderMode != RenderMode.WorldSpace)
            {
                // For Screen Space overlay or camera
                var screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
                var rectTransform = theClock.GetComponent<RectTransform>();
                if (rectTransform == null)
                    return;

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    rectTransform.parent as RectTransform,
                    screenCenter,
                    canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null,
                    out var localPoint);
                rectTransform.anchoredPosition = localPoint;
                // Optionally center align text
                var textMesh = theClock.GetComponent<TextMeshProUGUI>();
                if (textMesh != null)
                    textMesh.alignment = TextAlignmentOptions.Center;
            }
            else
            {
                // For WorldSpace or fallback
                var worldCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f,
                    Mathf.Abs(Camera.main.transform.position.z)));
                theClock.transform.position = new Vector3(worldCenter.x, worldCenter.y, theClock.transform.position.z);

                var textMesh = theClock.GetComponent<TextMeshProUGUI>();
                if (textMesh != null)
                    textMesh.alignment = TextAlignmentOptions.Center;
            }
        }

        private void FinalGoodEmployeeSequence()
        {
            Notify(GameEvents.StopDialogLine);
            PositionPlayerAt(LocationName.Home);
        }

        private void PositionPlayerAt(LocationName locationName)
        {
            var location = locations.First(item => item.location == locationName);
            player.transform.position = location.transform.position;
        }
    }
}