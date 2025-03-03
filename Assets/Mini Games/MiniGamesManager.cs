using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Mini_Games
{
    [Serializable]
    public enum MiniGameName
    {
        Reply,
        Flirt,
        Organize,
        File,
        Lunch
    }

    public class MiniGamesManager : ObserverSubject
    {
        [SerializeField] private MiniGameName[] instructions;
        [SerializeField] private Slider scoreSlider;
        [SerializeField] private TextMeshProUGUI inGameInstructionsText;
        [SerializeField] private GameObject inGameInstructions;

        private void Start()
        {
            inGameInstructions.SetActive(false);
            Invoke(nameof(SetRandomInstruction), 2f);
        }

        private void SetRandomInstruction()
        {
            if (instructions == null || instructions.Length == 0)
            {
                Debug.LogWarning("Instructions array is empty or null!");
                return;
            }

            inGameInstructions.SetActive(true);

            var randomIndex = Random.Range(0, instructions.Length);
            var selectedInstruction = instructions[randomIndex];

            if (inGameInstructions != null)
            {
                inGameInstructionsText.text = selectedInstruction + "!";
                TriggerMiniGame(selectedInstruction);
                Invoke(nameof(CloseInstruction), 5f);
            }
            else
            {
                Debug.LogWarning("inGameInstructions TextMeshProUGUI reference is null!");
            }
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.MiniGameClosed)
                Invoke(nameof(SetRandomInstruction), 0.5f);
        }

        private void CloseInstruction()
        {
            inGameInstructions.SetActive(false);
        }

        private void TriggerMiniGame(MiniGameName miniGameNameName)
        {
            Notify(GameEvents.MiniGameIndicationTrigger, miniGameNameName);
        }
    }
}