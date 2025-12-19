using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Mini_Games
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TheClockController : MonoBehaviour
    {
        [SerializeField] private float tickIntervalInSeconds = 1;
        private TextMeshProUGUI _clockTextMesh;

        public DateTime CurrentTime { get; set; } = new(2024, 12, 29, 7, 0, 0);

        public bool IsPaused { get; private set; }

        private void Start()
        {
            _clockTextMesh = GetComponent<TextMeshProUGUI>();
            SetClockString();
        }

        public event Action<DateTime> OnTimeReached;

        private IEnumerator Tick()
        {
            while (!IsPaused)
            {
                yield return new WaitForSeconds(tickIntervalInSeconds);
                AdvanceInMinutes(1);
            }
        }


        public void AdvanceInMinutes(int minutes)
        {
            var delta = TimeSpan.FromMinutes(minutes);

            if (IsPaused)
                return;

            CurrentTime += delta;
            SetClockString();
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

        public void StartTick()
        {
            StopAllCoroutines();
            Resume();
            StartCoroutine(Tick());
        }

        public void SetClockInterval(float interval)
        {
            tickIntervalInSeconds = interval;
        }

        private void SetClockString()
        {
            _clockTextMesh.text = CurrentTime.ToString("dddd HH:mm");
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.LineNarrationEnd)
                AdvanceInMinutes(2);
        }
    }
}