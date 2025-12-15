using UnityEngine;
using UnityEngine.UI;

namespace Mini_Games
{
    public class ChoiceButtonsDialog : MonoBehaviour
    {
        [SerializeField] private Button[] buttons;

        private void Start()
        {
            foreach (var t in buttons) t.onClick.AddListener(OnChoiceButtonClick);
        }

        private void OnChoiceButtonClick()
        {
        }

        private void PopulateChoices()
        {
            // for (var i = 0; i <= buttons.Length - 1; i++)
            // {
            //     var choice = buttons[i];
            //     buttons[i].GetComponentInChildren<TextMeshProUGUI>()
            //         .text = choice.text;
            // }
        }
    }
}