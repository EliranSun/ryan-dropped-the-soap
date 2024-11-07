using UnityEngine;
using UnityEngine.UI;

namespace Dialog.Scripts
{
    [RequireComponent(typeof(Collider2D))]
    public class Peephole : MonoBehaviour
    {
        [SerializeField] private Sprite personInFrontOfDoor;
        [SerializeField] private Sprite background;

        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image personImage;
        [SerializeField] private GameObject peepholeMask;
        [SerializeField] private Material peepholeMaterial;

        private void OnMouseDown()
        {
            ShowPeepholeView();
        }

        private void ShowPeepholeView()
        {
            // Set the sprites to the UI images
            backgroundImage.sprite = background;
            personImage.sprite = personInFrontOfDoor;

            // Apply the peephole material to the person image
            personImage.material = peepholeMaterial;

            // Enable the images and mask
            backgroundImage.gameObject.SetActive(true);
            personImage.gameObject.SetActive(true);
            peepholeMask.SetActive(true);
        }
    }
}