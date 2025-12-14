using System;
using System.Linq;
using TMPro;
using UnityEngine;

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
    public enum MiniGameLocation
    {
        OfficeOpenSpace,
        BossOffice,
        Home
    }

    [Serializable]
    public class MiniGamePosition
    {
        public MiniGameName name;
        public Transform position;
    }

    public class ClockifiedMiniGames : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI dayTimeTextMesh;
        [SerializeField] private GameObject player;
        [SerializeField] private int deadlineDays = 2;
        [SerializeField] private int deadlineHour = 18;
        [SerializeField] private float advanceIntervalInSeconds;
        [SerializeField] private MiniGamePosition[] miniGameLocations;
        private readonly string[] _dayNames = { "Sunday", "Monday", "Tuesday" };

        private Vector2 _currentMiniGamePosition;
        private int _currentTimeInMinutes = 9 * 60; // "Sunday, 8:00"
        private int _deadlineTime;

        private void Start()
        {
            _deadlineTime = CalcDeadline();
        }

        private int CalcDeadline()
        {
            return deadlineDays * 24 * 60 + deadlineHour * 60;
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name is GameEvents.MiniGameStart or GameEvents.StartMiniGames)
            {
                var isFirstTime = eventData.Name == GameEvents.StartMiniGames;
                AdvanceTime(isFirstTime ? 60 : 120);

                if (isFirstTime) return;

                var miniGameName = (MiniGameName)eventData.Data;
                var currentMiniGame = miniGameLocations.First(item => item.name == miniGameName);
                if (_currentMiniGamePosition == (Vector2)currentMiniGame.position.transform.position)
                    return;

                _currentMiniGamePosition = currentMiniGame.position.transform.position;
                player.transform.position = _currentMiniGamePosition;
            }
        }

        private void AdvanceTime(int minutes)
        {
            _currentTimeInMinutes += minutes;

            var dayNameIndex = _currentTimeInMinutes / 1440;
            if (dayNameIndex >= _dayNames.Length)
                return;

            var dayTime = _currentTimeInMinutes % 1440;
            var hours = dayTime / 60;
            var minutesInDay = dayTime % 60;
            var hoursLeft = (_deadlineTime - _currentTimeInMinutes) / 60;

            dayTimeTextMesh.text =
                $"{_dayNames[dayNameIndex]}, {hours:D2}:{minutesInDay:D2}; {hoursLeft}h left";
        }
    }
}