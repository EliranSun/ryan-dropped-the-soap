using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace common.scripts
{
    [Serializable]
    public class TransitionalElement
    {
        [SerializeField] public GameObject elementContainer;
        [SerializeField] public GameObject elementShade;
    }

    public sealed class LevelEvents : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Light2D globalLight;
        [SerializeField] private float transitionDuration = 2f;
        [SerializeField] private TransitionalElement transitionOutElement;
        [SerializeField] private TransitionalElement transitionInElement;
        [SerializeField] private KillingDependents killingDependents;

        public void OnNotify(GameEventData eventData)
        {
            print("LevelEvents OnNotify:  " + eventData);
            switch (eventData.name)
            {
                case GameEvents.TriggerAlarmClockStop:
                    mainCamera.gameObject.GetComponent<Zoom>().endSize = 9.28f;
                    globalLight.intensity = 1f;
                    break;

                case GameEvents.ExitApartment:
                    mainCamera.gameObject.transform.parent = player.transform;
                    StartCoroutine(SmoothCameraTransition(new Vector3(player.position.x, player.position.y, -10)));
                    mainCamera.gameObject.GetComponent<Zoom>().endSize = 8.5f;

                    StartCoroutine(FadeSprite(transitionInElement, false));
                    StartCoroutine(FadeSprite(transitionOutElement, true));
                    break;

                case GameEvents.EnterApartment:
                    mainCamera.gameObject.transform.parent = null;
                    StartCoroutine(SmoothCameraTransition(new Vector3(0, 3, -10)));
                    mainCamera.gameObject.GetComponent<Zoom>().endSize = 9.28f;

                    StartCoroutine(FadeSprite(transitionOutElement, false));
                    StartCoroutine(FadeSprite(transitionInElement, true));
                    break;

                // case GameEvents.EnterHallway:
                // {
                //     var temp = transitionInElement;
                //     transitionOutElement = transitionInElement;
                //     transitionInElement = temp;
                //     break;
                // }

                case GameEvents.KillDependents:
                    killingDependents.ActivateDependents();
                    break;
            }
        }

        private IEnumerator FadeSprite(TransitionalElement transitionalElement, bool fadeIn)
        {
            var shadeSpriteRenderer = transitionalElement.elementShade.GetComponent<SpriteRenderer>();
            var containerSpriteRenderer = transitionalElement.elementContainer.GetComponent<SpriteRenderer>();

            if (fadeIn) containerSpriteRenderer.GetComponent<Collider2D>().enabled = false;

            var startAlpha = fadeIn ? 0f : 1f;
            var endAlpha = fadeIn ? 1f : 0f;
            var elapsedTime = 0f;

            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                var alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / transitionDuration);
                shadeSpriteRenderer.color = new Color(0, 0, 0, alpha);
                yield return null;
            }

            // Ensure we end at exactly the target value
            shadeSpriteRenderer.color = new Color(0, 0, 0, endAlpha);
            containerSpriteRenderer.sortingOrder = fadeIn ? 3 : 7;

            yield return new WaitForSeconds(1);

            if (!fadeIn) containerSpriteRenderer.GetComponent<Collider2D>().enabled = true;
        }

        private IEnumerator SmoothCameraTransition(Vector3 targetPosition)
        {
            var elapsedTime = 0f;
            var startPosition = mainCamera.gameObject.transform.position;

            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                var t = elapsedTime / transitionDuration;
                mainCamera.gameObject.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                yield return null;
            }

            // Ensure we end at exactly the target position
            mainCamera.gameObject.transform.position = targetPosition;
        }
    }
}