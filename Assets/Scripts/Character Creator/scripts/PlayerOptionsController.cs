using System.Collections.Generic;
using Dialog;
using TMPro;
using UnityEngine;

namespace Character_Creator.scripts
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class PlayerOptionsController : ObserverSubject
    {
        private readonly List<NarrationDialogLine> _nextLines = new();
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

            // Check for numeric key presses from 1 to 9
            for (var i = 0; i < Mathf.Min(_nextLines.Count, 9); i++)
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    keyPressed = true;
                    if (_nextLines[i]) nextLine = _nextLines[i];
                }

            if (keyPressed) _playerOptionsText.text = "";

            if (nextLine)
                Notify(GameEvents.TriggerSpecificDialogLine, nextLine);
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
            if (playerOptions == null || playerOptions.Length == 0) return;

            _playerOptionsText.text = "";
            _nextLines.Clear();

            // Support up to 9 options (keys 1-9)
            var maxOptions = Mathf.Min(playerOptions.Length, 9);
            for (var i = 0; i < maxOptions; i++)
            {
                _nextLines.Add(playerOptions[i].next);
                _playerOptionsText.text += $"{i + 1}. {playerOptions[i].text}\r\n";
            }

            // If there are more options than we can display, add a note
            if (playerOptions.Length > 9)
                _playerOptionsText.text += "\n(Additional options not shown)";
        }
    }
}