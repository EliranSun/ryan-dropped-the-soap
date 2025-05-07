using UnityEngine;
using UnityEngine.Events;

namespace Elevator.scripts
{
    public class ButtonPhysics : MonoBehaviour
    {
        public UnityEvent onButtonPressed = new UnityEvent();

        [SerializeField] private Color highlightColor = Color.yellow;
        private Color originalColor;
        private Renderer buttonRenderer;
        private bool isHighlighted = false;

        private void Awake()
        {
            buttonRenderer = GetComponent<Renderer>();
            if (buttonRenderer != null)
            {
                originalColor = buttonRenderer.material.color;
            }
        }

        private void OnMouseEnter()
        {
            if (buttonRenderer != null)
            {
                isHighlighted = true;
                buttonRenderer.material.color = highlightColor;
            }
        }

        private void OnMouseExit()
        {
            if (buttonRenderer != null && isHighlighted)
            {
                isHighlighted = false;
                buttonRenderer.material.color = originalColor;
            }
        }

        private void OnMouseDown()
        {
            Debug.Log($"Button {gameObject.name} clicked (OnMouseDown)");
            onButtonPressed.Invoke();
        }
    }
}