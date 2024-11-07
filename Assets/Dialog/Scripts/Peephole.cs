using System.Collections;
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

        private bool _isOpen;

        private void OnMouseDown()
        {
            if (_isOpen) HidePeepholeView();
            else ShowPeepholeView();
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

            StartCoroutine(FlipPersonImage());

            _isOpen = true;
        }

        private void HidePeepholeView()
        {
            backgroundImage.gameObject.SetActive(false);
            personImage.gameObject.SetActive(false);
            peepholeMask.SetActive(false);

            _isOpen = false;
        }

        private IEnumerator FlipPersonImage()
        {
            yield return new WaitForSeconds(3.5f);
            var originalScale = personImage.transform.localScale;
            personImage.transform.localScale =
                new Vector3(originalScale.x * -1, originalScale.y, originalScale.z); // Flip horizontally by scaling
        }
    }
}