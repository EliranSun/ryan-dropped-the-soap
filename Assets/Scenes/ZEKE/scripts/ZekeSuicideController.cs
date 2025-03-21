using UnityEngine;
using System.Collections;
using Object.Scripts;
using Dialog.Scripts;

namespace Scenes.ZEKE.scripts
{
    public class ZekeSuicideController : ObserverSubject
    {
        [SerializeField] private GameObject mainCamera;
        [SerializeField] private GameObject suicideCamera;
        [SerializeField] private GameObject zeke;
        [SerializeField] private Transform initPosition;
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

            float speed = 2.0f;
            Vector3 targetPosition = zeke.transform.position;
            targetPosition.y = suicideCamera.transform.position.y; // move only the x axis

            while (Vector3.Distance(suicideCamera.transform.position, targetPosition) > 5f)
            {
                float step = speed * Time.deltaTime;
                suicideCamera.transform.position = Vector3.MoveTowards(suicideCamera.transform.position, targetPosition, step);
                yield return null;
            }
        }

        private IEnumerator FadeOutBlackness()
        {
            float speed = 0.1f;
            float targetAlpha = 0.5f;
            float currentAlpha = blackness.GetComponent<SpriteRenderer>().color.a;

            while (currentAlpha > targetAlpha)
            {
                currentAlpha -= speed * Time.deltaTime;
                blackness.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, currentAlpha);
                yield return null;
            }
        }
    }
}