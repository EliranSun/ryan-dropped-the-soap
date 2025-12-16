using System.Collections;
using UnityEngine;

namespace Common.scripts
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class HighlightObject : MonoBehaviour
    {
        [SerializeField] private float pulseSpeed;
        [SerializeField] private float pulseScaleMin;
        [SerializeField] private float pulseScaleMax;
        [SerializeField] private Color highlightColor;
        private Vector3 _originalScale;
        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _originalScale = gameObject.transform.localScale;

            StartCoroutine(HighlightNpcCoroutine());
        }

        private IEnumerator HighlightNpcCoroutine()
        {
            var originalColor = _spriteRenderer != null ? _spriteRenderer.color : Color.white;

            var time = 0f;

            while (true)
            {
                time += Time.deltaTime * pulseSpeed;

                // Calculate pulsing scale using sine wave
                var scaleFactor = Mathf.Lerp(pulseScaleMin, pulseScaleMax, (Mathf.Sin(time) + 1f) / 2f);
                gameObject.transform.localScale = _originalScale * scaleFactor;

                // Calculate flashing color using sine wave
                if (_spriteRenderer != null)
                {
                    var colorIntensity = (Mathf.Sin(time * 1.5f) + 1f) / 2f; // Slightly faster color pulse
                    _spriteRenderer.color = Color.Lerp(originalColor, highlightColor, colorIntensity);
                }

                yield return null;
            }
        }
    }
}