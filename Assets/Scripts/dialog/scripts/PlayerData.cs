using System;
using UnityEngine;

namespace dialog.scripts
{
    public class PlayerData
    {
        private string feeling;
        private CharacterType gender;
        private string name;
        private string partner;

        private PlayerData()
        {
        }

        public static PlayerData Instance { get; } = new();

        public static string GetPlayerName()
        {
            var playerName = string.IsNullOrEmpty(Instance.name)
                ? PlayerPrefs.GetString(PlayerDataEnum.Name.ToString())
                : Instance.name;

            return UpperFirst(playerName);
        }

        public static string GetPartnerName()
        {
            var partnerName = string.IsNullOrEmpty(Instance.partner)
                ? PlayerPrefs.GetString(PlayerDataEnum.Partner.ToString())
                : Instance.partner;

            return UpperFirst(partnerName);
        }

        private static string UpperFirst(string value)
        {
            if (value == "") return "";

            return $"{char.ToUpper(value[0])}{value.Substring(1)}";
        }

        public static CharacterType GetPlayerGender()
        {
            var savedPlayerGender = PlayerPrefs.GetString(PlayerDataEnum.Gender.ToString());

            if (savedPlayerGender != "")
                return GetGenderByString(savedPlayerGender);

            return CharacterType.None;
        }

        public static CharacterType GetGenderByString(string value)
        {
            return value.ToLower() switch
            {
                "" => CharacterType.None,
                "male" => CharacterType.Male,
                "female" => CharacterType.Female,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(gender),
                    $"Not expected direction value: {value}"
                )
            };
        }
    }
}