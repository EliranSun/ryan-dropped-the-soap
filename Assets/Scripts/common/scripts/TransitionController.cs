using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace common.scripts
{
    [RequireComponent(typeof(Image))]
    public class TransitionController : MonoBehaviour
    {
        [SerializeField] private Color targetColor = Color.black;
        private Image _overlayImage;

        private void Start()
        {
            _overlayImage = GetComponent<Image>();
        }

        private void OnEnable()
        {
            _overlayImage = GetComponent<Image>();
        }

        public void FadeIn()
        {
            StopAllCoroutines();
            StartCoroutine(Fade(true));
        }

        public void FadeOut()
        {
            StopAllCoroutines();
            StartCoroutine(Fade(false));
        }

        public void FadeInOut(float duration = 0.3f)
        {
            StopAllCoroutines();
            StartCoroutine(Fade(true));
            Invoke(nameof(FadeOut), duration);
        }

        public void FadeOutIn(float duration = 0.3f)
        {
            StopAllCoroutines();
            StartCoroutine(Fade(false));
            Invoke(nameof(FadeIn), duration);
        }

        private IEnumerator Fade(bool fadeIn)
        {
            if (!_overlayImage)
            {
                print("No overlay image");
                yield break;
            }

            var startAlpha = fadeIn ? 0f : 1f;
            var endAlpha = fadeIn ? 1f : 0f;
            var elapsedTime = 0f;
            var transitionDuration = 0.2f;

            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                var alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / transitionDuration);

                _overlayImage.color = new Color(targetColor.r, targetColor.g, targetColor.b, alpha);
                yield return null;
            }

            // Ensure the final alpha is set exactly
            _overlayImage.color = new Color(targetColor.r, targetColor.g, targetColor.b, endAlpha);

            yield return new WaitForEndOfFrame();
        }
    }
}