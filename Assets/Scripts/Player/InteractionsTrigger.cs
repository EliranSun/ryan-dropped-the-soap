using TMPro;
using UnityEngine;

namespace Player
{
    public class InteractionsTrigger : MonoBehaviour
    {
        [SerializeField] private TextMeshPro interactionText;

        private void Start()
        {
            interactionText.text = "!!!";
        }

        // Update is called once per frame
        private void Update()
        {
        }
    }
}