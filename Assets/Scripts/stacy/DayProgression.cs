using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace stacy
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class DayProgression : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI dayIndicatorText;
        [SerializeField] private Image panel;
        private int _dayStart = 8;


        private void Start()
        {
            dayIndicatorText = GetComponent<TextMeshProUGUI>();
            dayIndicatorText.text = $"Day {_dayStart}";

            Invoke(nameof(HideUI), 5f);
        }

        private void HideUI()
        {
            panel.gameObject.SetActive(false);
            dayIndicatorText.gameObject.SetActive(false);
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.Dead)
            {
                _dayStart++;
                dayIndicatorText.text = $"Day {_dayStart}";
            }
        }
    }
}