using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mini_Games
{
    public class MiniGamesManager : MonoBehaviour
    {
        [SerializeField] private string[] instructions;
        [SerializeField] private Slider scoreSlider;
        [SerializeField] private TextMeshProUGUI inGameInstructions;

        private void Start()
        {
            SetRandomInstruction();
        }

        private void SetRandomInstruction()
        {
            if (instructions == null || instructions.Length == 0)
            {
                Debug.LogWarning("Instructions array is empty or null!");
                return;
            }

            var randomIndex = Random.Range(0, instructions.Length);
            var selectedInstruction = instructions[randomIndex];

            if (inGameInstructions != null)
                inGameInstructions.text = selectedInstruction;
            else
                Debug.LogWarning("inGameInstructions TextMeshProUGUI reference is null!");
        }
    }
}