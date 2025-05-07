using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace common.scripts
{
    [RequireComponent(typeof(Image))]
    public class TransitionController : MonoBehaviour
    {
        private Image _overlayImage;

        private void Start()
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

        public void FadeInOut()
        {
            StopAllCoroutines();
            StartCoroutine(Fade(true));
            Invoke(nameof(FadeOut), 0.3f);
        }

        public void FadeOutIn()
        {
            StopAllCoroutines();
            StartCoroutine(Fade(false));
            Invoke(nameof(FadeIn), 0.3f);
        }

        private IEnumerator Fade(bool fadeIn)
        {
            // var fadeIn = _overlayImage.color.a == 0;

            var startAlpha = fadeIn ? 0f : 1f;
            var endAlpha = fadeIn ? 1f : 0f;
            var elapsedTime = 0f;
            var transitionDuration = 0.2f;

            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                var alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / transitionDuration);
                _overlayImage.color = new Color(0f, 0f, 0f, alpha);
                yield return null;
            }

            // Ensure the final alpha is set exactly
            _overlayImage.color = new Color(0f, 0f, 0f, endAlpha);

            yield return new WaitForEndOfFrame();
        }
    }
}