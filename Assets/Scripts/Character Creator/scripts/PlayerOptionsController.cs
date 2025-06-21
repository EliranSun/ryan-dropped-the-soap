using Dialog;
using TMPro;
using UnityEngine;

namespace Character_Creator.scripts
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class PlayerOptionsController : ObserverSubject
    {
        private readonly NarrationDialogLine[] _nextLines = new NarrationDialogLine[4];
        private TextMeshProUGUI _playerOptionsText;

        private void Start()
        {
            _playerOptionsText = GetComponent<TextMeshProUGUI>();
            _playerOptionsText.text = "";
        }

        private void Update()
        {
            NarrationDialogLine nextLine = null;

            if (Input.GetKeyDown(KeyCode.Alpha1) && _nextLines[0] != null) nextLine = _nextLines[0];
            if (Input.GetKeyDown(KeyCode.Alpha2) && _nextLines[1] != null) nextLine = _nextLines[1];
            if (Input.GetKeyDown(KeyCode.Alpha3) && _nextLines[2] != null) nextLine = _nextLines[2];
            if (Input.GetKeyDown(KeyCode.Alpha4) && _nextLines[3] != null) nextLine = _nextLines[3];

            if (nextLine)
            {
                Notify(GameEvents.TriggerSpecificDialogLine, nextLine);
                _playerOptionsText.text = "";
            }
        }


        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.LineNarrationEnd)
            {
                var playerOptions = eventData.Data.GetType().GetProperty("playerOptions");
                if (playerOptions == null) return;
                var playerOptionsValue = (PlayerChoice[])playerOptions.GetValue(eventData.Data);

                HandlePlayerOptions(playerOptionsValue);
            }
        }

        private void HandlePlayerOptions(PlayerChoice[] playerOptions)
        {
            if (playerOptions == null || playerOptions.Length == 0)
                return;

            _playerOptionsText.text = "";

            for (var i = 0; i < playerOptions.Length; i++)
            {
                _nextLines[i] = playerOptions[i].next;
                _playerOptionsText.text += $"{i + 1}. {playerOptions[i].text}\r\n";
            }
        }
    }
}