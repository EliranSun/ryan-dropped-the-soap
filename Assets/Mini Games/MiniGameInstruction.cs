using TMPro;
using UnityEngine;

namespace Mini_Games
{
    public class MiniGameInstruction : MonoBehaviour
    {
        [SerializeField] private int hideAfter = 3;
        [SerializeField] private GameObject container;
        [SerializeField] private TextMeshProUGUI textMesh;

        private void Start()
        {
            Hide();
        }

        public void SetInstructions(string instructions)
        {
            textMesh.text = instructions.Trim();
            container.SetActive(true);
            Invoke(nameof(Hide), hideAfter);
        }

        private void Hide()
        {
            container.SetActive(false);
        }
    }
}