using System.Collections;
using Dialog.Scripts;
using UnityEngine;

namespace Scenes.ZEKE.scripts
{
    public class ZekeSuicideController : ObserverSubject
    {
        [SerializeField] private GameObject mainCamera;
        [SerializeField] private GameObject suicideCamera;
        [SerializeField] private GameObject zeke;
        [SerializeField] private NarrationDialogLine initLine;
        [SerializeField] private GameObject blackness;

        private void OnEnable()
        {
            mainCamera.SetActive(false);
            suicideCamera.SetActive(true);

            Invoke(nameof(TriggerInitLine), 1f);
            Invoke(nameof(TriggerCameraMovement), 4f);
            Invoke(nameof(TriggerFadeOutBlackness), 4f);
        }

        private void TriggerCameraMovement()
        {
            StartCoroutine(MoveCamera());
        }

        private void TriggerFadeOutBlackness()
        {
            StartCoroutine(FadeOutBlackness());
        }

        private void TriggerInitLine()
        {
            Notify(GameEvents.TriggerSpecificDialogLine, initLine);
        }

        private IEnumerator MoveCamera()
        {
            yield return new WaitForSeconds(1f);

            var speed = 2.0f;
            var targetPosition = zeke.transform.position;
            targetPosition.y = suicideCamera.transform.position.y; // move only the x axis

            while (Vector3.Distance(suicideCamera.transform.position, targetPosition) > 5f)
            {
                var step = speed * Time.deltaTime;
                suicideCamera.transform.position =
                    Vector3.MoveTowards(suicideCamera.transform.position, targetPosition, step);
                yield return null;
            }

            Invoke(nameof(EndScene), 5f);
        }

        private void EndScene()
        {
            Notify(GameEvents.ZekeSuicideEnd);
        }

        private IEnumerator FadeOutBlackness()
        {
            var speed = 0.1f;
            var targetAlpha = 0.5f;
            var currentAlpha = blackness.GetComponent<SpriteRenderer>().color.a;

            while (currentAlpha > targetAlpha)
            {
                currentAlpha -= speed * Time.deltaTime;
                blackness.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, currentAlpha);
                yield return null;
            }
        }
    }
}