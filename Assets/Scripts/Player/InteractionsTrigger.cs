using TMPro;
using UnityEngine;

namespace Player
{
    public class InteractionsTrigger : ObserverSubject
    {
        [SerializeField] private TextMeshPro interactionText;
        private GameObject _interactedObject;

        private void Start()
        {
            interactionText.text = "";
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X) && _interactedObject)
                Notify(GameEvents.PlayerInteraction, _interactedObject);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            InstructBasedOnTag(other.gameObject);
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            ResetText(other.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            InstructBasedOnTag(other.gameObject);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            ResetText(other.gameObject);
        }

        private void ResetText(GameObject other)
        {
            if (other.CompareTag("Apartment Door") ||
                other.CompareTag("Door") ||
                other.CompareTag("Building Entrance") ||
                other.CompareTag("NPC"))
            {
                interactionText.text = "";
                _interactedObject = null;
            }
        }

        private void InstructBasedOnTag(GameObject other)
        {
            if (other.CompareTag("Building Entrance")) interactionText.text = "Press X to enter";
            if (other.CompareTag("Apartment Door")) interactionText.text = "Press X to knock";
            if (other.CompareTag("NPC")) interactionText.text = "Press X to talk";
            if (other.CompareTag("Door"))
            {
                interactionText.text = "Press X to open door";
                _interactedObject = other.gameObject;
            }
        }
    }
}