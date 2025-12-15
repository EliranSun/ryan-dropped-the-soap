using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mini_Games.Snooze
{
    public class SnoozeMiniGame : ObserverSubject
    {
        [SerializeField] private int startTimeInMinutes = 23 * 60 + 30; // 23:30
        [SerializeField] private int wakeUpTime = 8 * 60; // 8:00
        [SerializeField] private float clockSpeedInSeconds;
        [SerializeField] private GameObject gameContainer;
        [SerializeField] private Sprite asleepFace;
        [SerializeField] private Sprite awakeFace;
        [SerializeField] private Image faceImage;
        [SerializeField] private TextMeshProUGUI clockTextMesh;
        [SerializeField] private TextMeshProUGUI instructionText;
        [SerializeField] private AudioClip instructionAudio;
        [SerializeField] private AudioClip loseStateAudio;
        [SerializeField] private AudioClip winStateAudio;
        private bool _isSnoozeHit;

        private void Start()
        {
            faceImage.sprite = asleepFace;
            instructionText.text = $"I need to wake up exactly at {GetTimeString(wakeUpTime)}...";
            StartCoroutine(AdvanceTime());

            gameContainer.SetActive(false);
        }

        private void SetTime()
        {
            clockTextMesh.text = GetTimeString(startTimeInMinutes);
        }

        private string GetTimeString(int timeInMinutes = 0)
        {
            var dayWrap = timeInMinutes % 1440;
            var hour = dayWrap / 60;
            var minutes = dayWrap % 60;
            var minutesFormatted = minutes < 10 ? $"0{minutes}" : minutes.ToString();

            print($"Time {hour}:{minutes}; {timeInMinutes}");

            return $"{hour}:{minutesFormatted}";
        }

        private IEnumerator AdvanceTime()
        {
            while (!_isSnoozeHit)
            {
                /*
                 * Compute totalMinutes % (24 * 60) to wrap around the day,
                 * then derive hours/minutes from that value (use integer division for hours, remainder for minutes).
                 */
                SetTime();
                startTimeInMinutes++;
                yield return new WaitForSeconds(1 / clockSpeedInSeconds);
            }
        }

        public void OnSnoozeClick()
        {
            _isSnoozeHit = true;

            faceImage.sprite = awakeFace;

            var adjustedTime = startTimeInMinutes - 1;
            print($"{adjustedTime} vs. {wakeUpTime}");

            var isSuccess = adjustedTime == wakeUpTime;

            if (isSuccess) instructionText.text = "Right on time........";
            if (adjustedTime > wakeUpTime) instructionText.text = "FUCK!!!!!!!!!!!";
            if (adjustedTime < wakeUpTime) instructionText.text = "What is the meaning of my life...?";

            OnGameEnd(isSuccess);
        }

        private void OnGameEnd(bool isSuccess)
        {
            if (isSuccess)
                Notify(GameEvents.MiniGameWon, 1);
            else
                Notify(GameEvents.MiniGameLost, 0);

            gameContainer.SetActive(false);
        }
    }
}