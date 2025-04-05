using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DayProgression : MonoBehaviour
{
    private TextMeshProUGUI _dayIndicatorText;
    private int _dayStart = 8;


    private void Start()
    {
        _dayIndicatorText = GetComponent<TextMeshProUGUI>();
        _dayIndicatorText.text = $"Day {_dayStart}";
    }

    public void OnNotify(GameEventData eventData)
    {
        if (eventData.Name == GameEvents.Dead) {
            _dayStart++;
            _dayIndicatorText.text = $"Day {_dayStart}";
        }
    }
}
