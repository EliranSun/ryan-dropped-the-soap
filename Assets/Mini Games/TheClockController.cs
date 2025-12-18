using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Mini_Games
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TheClockController : MonoBehaviour
    {
        private TextMeshProUGUI _clockTextMesh;

        public DateTime CurrentTime { get; set; } = new(2024, 12, 29, 7, 0, 0);

        public bool IsPaused { get; private set; }

        private void Start()
        {
            _clockTextMesh = GetComponent<TextMeshProUGUI>();
            StartCoroutine(Tick());
        }

        public event Action<DateTime> OnTimeReached;

        private IEnumerator Tick()
        {
            while (!IsPaused)
            {
                yield return new WaitForSeconds(1);
                AdvanceInMinutes(1);
                SetClockString();
                print($"tick {IsPaused}");
            }
        }


        public void AdvanceInMinutes(int minutes)
        {
            var delta = TimeSpan.FromMinutes(minutes);

            if (IsPaused)
                return;

            CurrentTime += delta;
            OnTimeReached?.Invoke(CurrentTime);
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void Resume()
        {
            IsPaused = false;
        }

        private void SetClockString()
        {
            _clockTextMesh.text = CurrentTime.ToString("dddd HH:mm");
            // if (dayNameIndex >= _dayNames.Length)
            //     return;
            //
            // var dayTime = _currentTimeInMinutes % 1440;
            // var hours = dayTime / 60;
            // var minutesInDay = dayTime % 60;
            // var hoursLeft = (_deadlineTime - _currentTimeInMinutes) / 60;
            // var theClockTextMesh = theClock.GetComponent<TextMeshProUGUI>();
            // theClockTextMesh.text = $"{_dayNames[dayNameIndex]}, {hours:D2}:{minutesInDay:D2}";
            // if (!_suicideChoicesStarted) theClockTextMesh.text += $"; {hoursLeft}h left";
        }
    }
}