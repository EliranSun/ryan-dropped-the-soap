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
            var keyPressed = false;

            for (var i = 0; i < 4; i++)
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    keyPressed = true;
                    if (_nextLines[i])
                        nextLine = _nextLines[i];
                }

            if (keyPressed)
                _playerOptionsText.text = "";

            if (nextLine) Notify(GameEvents.TriggerSpecificDialogLine, nextLine);
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