using System.Collections;
using TMPro;
using UnityEngine;

public class Timer : ObserverSubject {
    [SerializeField] private int timeInSeconds = 60;
    [SerializeField] private TextMeshProUGUI timerText;

    private void Start() {
        Notify(GameEvents.TimerUpdate, timeInSeconds);
        UpdateTimerText();
        StartCoroutine(CountDown());
    }

    private string PrefixTime(float time) {
        return time < 10 ? $"0{time}" : $"{time}";
    }

    private IEnumerator CountDown() {
        while (timeInSeconds > 0) {
            yield return new WaitForSeconds(1);
            timeInSeconds--;
            UpdateTimerText();
        }
    }

    private void UpdateTimerText() {
        var minutes = Mathf.Floor((float)timeInSeconds / 60);
        var reminderSeconds = Mathf.Round((float)timeInSeconds % 60);

        timerText.text = $"{PrefixTime(minutes)}:{PrefixTime(reminderSeconds)}";
    }
}