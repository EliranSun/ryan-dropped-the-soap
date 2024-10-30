using System;
using Dialog.Scripts;
using TMPro;
using UnityEngine;

namespace Character_Creator.scripts
{
    public class PlayerInfoInput : MonoBehaviour
    {
        [SerializeField] public PlayerDataEnum type;
        [SerializeField] public PlayerDataOption option;
        private string _buttonText;
        private TMP_InputField _inputField;

        private void Start()
        {
            _inputField = GetComponentInChildren<TMP_InputField>();
            if (_inputField != null)
                _inputField.onValueChanged.AddListener(OnInput);
        }

        public void OnMouseDown()
        {
            if (option == PlayerDataOption.None)
                return;

            var key = type.ToString();
            var lowerValue = option.ToString().ToLower();

            print($"Setting {key} to {lowerValue}");
            PlayerPrefs.SetString(key, lowerValue);
        }

        public void OnInput(string value)
        {
            // foreach (var textMeshProUGUI in gameObject.GetComponentsInChildren<TextMeshProUGUI>())
            //     if (textMeshProUGUI.gameObject.name == "PlayerInfoTextValue")
            //         _value = textMeshProUGUI.text;

            if (type == PlayerDataEnum.None)
                return;

            if (string.IsNullOrEmpty(value.Trim()) || value.Trim().Length <= 1)
                throw new Exception("must provide value for player prefs");


            var key = type.ToString();
            var lowerValue = value.ToLower();

            print($"Setting {key} to {lowerValue}");
            PlayerPrefs.SetString(key, lowerValue);
        }
    }
}