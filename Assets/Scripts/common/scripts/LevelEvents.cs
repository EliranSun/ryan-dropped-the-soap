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

    public sealed class LevelEvents : ObserverSubject
    {
        [SerializeField] private Transform player;
        [SerializeField] private Transform charlotte;
        [SerializeField] private GameObject ship;
        [SerializeField] private Transform insideSewerPosition;
        [SerializeField] private Transform nearShipPosition;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Light2D globalLight;
        [SerializeField] private float transitionDuration = 2f;
        [SerializeField] private GameObject[] transitionOutElements;
        [SerializeField] private GameObject[] transitionInElements;
        [SerializeField] private KillingDependents killingDependents;

        private void Start()
        {
            var zekeSceneEnded = PlayerPrefs.GetString("Zeke Scene End");
            if (zekeSceneEnded == "")
                return;

            print("@@@@@@@@@@ Zeke ended");
            MoveToBoat();
            Invoke(nameof(BoatStart), 5f);
        }

        public void OnNotify(GameEventData eventData)
        {
            switch (eventData.Name)
            {
                case GameEvents.TriggerAlarmClockStop:
                    mainCamera.gameObject.GetComponent<Zoom>().endSize = 9.28f;
                    globalLight.intensity = 1f;
                    break;

                case GameEvents.ExitApartment:
                    // mainCamera.gameObject.transform.parent = player.transform;
                    // StartCoroutine(SmoothCameraTransition(new Vector3(player.position.x, player.position.y, -10)));
                    // mainCamera.gameObject.GetComponent<Zoom>().endSize = 8.5f;

                    foreach (var transitionInElement in transitionInElements)
                        StartCoroutine(FadeSprite(transitionInElement, false));

                    foreach (var transitionOutElement in transitionOutElements)
                        StartCoroutine(FadeSprite(transitionOutElement, true));
                    break;

                case GameEvents.EnterApartment:
                    mainCamera.gameObject.transform.parent = null;
                    StartCoroutine(SmoothCameraTransition(new Vector3(0, 3, -10)));
                    mainCamera.gameObject.GetComponent<Zoom>().endSize = 9.28f;

                    foreach (var transitionOutElement in transitionOutElements)
                        StartCoroutine(FadeSprite(transitionOutElement, false));

                    foreach (var transitionInElement in transitionInElements)
                        StartCoroutine(FadeSprite(transitionInElement, true));
                    break;

                case GameEvents.KillDependents:
                    killingDependents.ActivateDependents();
                    break;

                case GameEvents.CharlotteBeachDialogInit:
                    charlotte.transform.position = new Vector2(
                        nearShipPosition.position.x,
                        nearShipPosition.position.y
                    );
                    player.transform.position = new Vector2(
                        insideSewerPosition.position.x + 1f,
                        insideSewerPosition.position.y
                    );
                    break;

                case GameEvents.CharlotteBeachDialogEnd:
                    MoveToBoat();
                    Invoke(nameof(BoatStart), 5f);
                    break;

                case GameEvents.ZoomOut:
                    StartCoroutine(SmoothCameraZoom(7.4f));
                    break;

                case GameEvents.Dead:
                    break;
            }
        }

        private void MoveToBoat()
        {
            player.transform.parent = ship.transform;
            charlotte.transform.parent = ship.transform;

            charlotte.transform.position = new Vector2(
                ship.transform.position.x - 1,
                ship.transform.position.y
            );
            player.transform.position = new Vector2(
                ship.transform.position.x - 5,
                ship.transform.position.y
            );
        }

        private void BoatStart()
        {
            Notify(GameEvents.BoatStart);
        }

        private IEnumerator FadeSprite(GameObject transitionalElement, bool fadeIn)
        {
            // var shadeSpriteRenderer = transitionalElement.elementShade.GetComponent<SpriteRenderer>();
            var containerSpriteRenderer = transitionalElement.GetComponent<SpriteRenderer>();

            if (fadeIn) containerSpriteRenderer.GetComponent<Collider2D>().enabled = false;

            var startAlpha = fadeIn ? 0f : 1f;
            var endAlpha = fadeIn ? 1f : 0f;
            var elapsedTime = 0f;

            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                var alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / transitionDuration);
                containerSpriteRenderer.color = new Color(1, 1, 1, alpha);
                yield return null;
            }

            // Ensure we end at exactly the target value
            containerSpriteRenderer.color = new Color(1, 1, 1, endAlpha);
            containerSpriteRenderer.sortingOrder = fadeIn ? 3 : 7;

            yield return new WaitForSeconds(1);

            if (!fadeIn) 
                containerSpriteRenderer.GetComponent<Collider2D>().enabled = true;
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

        private IEnumerator SmoothCameraZoom(float targetSize)
        {
            var elapsedTime = 0f;
            var startSize = mainCamera.gameObject.GetComponent<Zoom>().endSize;

            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                var t = elapsedTime / transitionDuration;
                mainCamera.gameObject.GetComponent<Zoom>().endSize = Mathf.Lerp(startSize, targetSize, t);
                yield return null;
            }

            // Ensure we end at exactly the target size
            mainCamera.gameObject.GetComponent<Zoom>().endSize = targetSize;
        }
    }
}