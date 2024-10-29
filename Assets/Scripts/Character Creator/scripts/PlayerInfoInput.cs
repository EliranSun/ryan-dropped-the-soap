using System;
using Dialog.Scripts;
using TMPro;
using UnityEngine;

namespace Character_Creator.scripts
{
    public class PlayerInfoInput : MonoBehaviour
    {
        [SerializeField] public PlayerDataEnum type;
        private string value;

        public void OnInput()
        {
            foreach (var textMeshProUGUI in gameObject.GetComponentsInChildren<TextMeshProUGUI>())
                if (textMeshProUGUI.gameObject.name == "PlayerInfoTextValue")
                    value = textMeshProUGUI.text;

            if (string.IsNullOrEmpty(value.Trim()) || value.Trim().Length <= 1)
                throw new Exception("must provide value for player prefs");

            print($"Setting {type.ToString()} to {value.ToLower()}");

            var key = type.ToString();
            var lowerValue = value.ToLower();

            PlayerPrefs.SetString(key, lowerValue);
        }
    }
}