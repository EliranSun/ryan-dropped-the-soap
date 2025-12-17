using System.Collections;
using UnityEngine;

namespace Mini_Games
{
    public class MiniGameAnimation : MonoBehaviour
    {
        [Header("Animation Settings")] [SerializeField]
        private float animationDuration = 1f;

        [SerializeField] private float jumpHeight = 0.5f;
        [SerializeField] private float shakeIntensity = 0.2f;

        private IEnumerator PlayWinAnimation(GameObject target)
        {
            if (target == null) yield break;

            yield return new WaitForSeconds(1);

            var targetTransform = target.transform;
            var originalPosition = targetTransform.position;
            var originalScale = targetTransform.localScale;

            // Happy celebration sequence: flip, jump, scale bounce
            var elapsedTime = 0f;

            // Phase 1: Quick scale up and flip
            while (elapsedTime < animationDuration * 0.3f)
            {
                var progress = elapsedTime / (animationDuration * 0.3f);
                var scaleMultiplier = 1f + Mathf.Sin(progress * Mathf.PI) * 0.3f;
                targetTransform.localScale = originalScale * scaleMultiplier;

                // Flip effect by scaling X negative and back
                if (progress < 0.5f)
                    targetTransform.localScale = new Vector3(-originalScale.x * scaleMultiplier,
                        originalScale.y * scaleMultiplier,
                        originalScale.z * scaleMultiplier);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Reset scale
            targetTransform.localScale = originalScale;

            // Phase 2: Jump animation
            elapsedTime = 0f;
            while (elapsedTime < animationDuration * 0.7f)
            {
                var progress = elapsedTime / (animationDuration * 0.7f);
                var jumpOffset = Mathf.Sin(progress * Mathf.PI * 2) * jumpHeight;
                var scaleOffset = 1f + Mathf.Sin(progress * Mathf.PI * 4) * 0.1f;

                targetTransform.position = originalPosition + Vector3.up * jumpOffset;
                targetTransform.localScale = originalScale * scaleOffset;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Reset to original state
            targetTransform.position = originalPosition;
            targetTransform.localScale = originalScale;
        }

        private IEnumerator PlayLoseAnimation(GameObject target)
        {
            if (target == null) yield break;

            yield return new WaitForSeconds(1);

            var targetTransform = target.transform;
            var originalPosition = targetTransform.position;
            var originalScale = targetTransform.localScale;

            // Sad defeat sequence: shake, shrink, droop
            var elapsedTime = 0f;

            // Phase 1: Shake and shrink
            while (elapsedTime < animationDuration * 0.5f)
            {
                var progress = elapsedTime / (animationDuration * 0.5f);

                // Shake effect - random offset
                var shakeOffset = new Vector3(
                    Random.Range(-shakeIntensity, shakeIntensity),
                    Random.Range(-shakeIntensity * 0.5f, shakeIntensity * 0.5f),
                    0f
                );

                // Shrinking scale
                var shrinkAmount = 1f - progress * 0.2f;
                targetTransform.localScale = originalScale * shrinkAmount;
                targetTransform.position = originalPosition + shakeOffset;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Phase 2: Droop down (sad slump)
            elapsedTime = 0f;
            while (elapsedTime < animationDuration * 0.5f)
            {
                var progress = elapsedTime / (animationDuration * 0.5f);

                // Droop effect - scale Y down and position slightly down
                var droopScale = 1f - progress * 0.3f;
                var droopOffset = -progress * 0.1f;

                targetTransform.localScale = new Vector3(
                    originalScale.x * 0.8f,
                    originalScale.y * droopScale,
                    originalScale.z * 0.8f
                );
                targetTransform.position = originalPosition + Vector3.up * droopOffset;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Hold the sad pose briefly
            yield return new WaitForSeconds(0.3f);

            // Slowly return to original state
            elapsedTime = 0f;
            var currentScale = targetTransform.localScale;
            var currentPos = targetTransform.position;

            while (elapsedTime < 0.5f)
            {
                var progress = elapsedTime / 0.5f;

                targetTransform.localScale = Vector3.Lerp(currentScale, originalScale, progress);
                targetTransform.position = Vector3.Lerp(currentPos, originalPosition, progress);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure exact reset
            targetTransform.position = originalPosition;
            targetTransform.localScale = originalScale;
        }
    }
}