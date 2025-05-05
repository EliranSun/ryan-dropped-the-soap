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

        // [SerializeField] private GameObject[] transitionOutElements;
        // [SerializeField] private GameObject[] transitionInElements;

        [Header("Apartment")] [SerializeField] private KillingDependents killingDependents;

        [SerializeField] private GameObject hallwayShade;
        [SerializeField] private GameObject apartmentsShade;
        [SerializeField] private GameObject[] hallways;
        [SerializeField] private GameObject apartmentsBoundaries;
        [SerializeField] private GameObject hallwayBoundaries;


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
                    print("EXIT APARTMENT");
                    // mainCamera.gameObject.transform.parent = player.transform;
                    // StartCoroutine(SmoothCameraTransition(new Vector3(player.position.x, player.position.y, -10)));
                    // mainCamera.gameObject.GetComponent<Zoom>().endSize = 8.5f;

                    // foreach (var transitionInElement in transitionInElements)
                    //     // StartCoroutine(FadeSprite(transitionInElement, false));
                    //     transitionInElement.SetActive(false);
                    //
                    // foreach (var transitionOutElement in transitionOutElements)
                    //     // StartCoroutine(FadeSprite(transitionOutElement, true));
                    //     transitionOutElement.SetActive(true);
                    apartmentsShade.SetActive(true);
                    hallwayShade.SetActive(false);
                    foreach (var hallway in hallways)
                        hallway.GetComponent<SpriteRenderer>().sortingOrder = 2;

                    foreach (var boundary in apartmentsBoundaries.GetComponents<Collider2D>())
                        boundary.enabled = false;
                    foreach (var boundary in hallwayBoundaries.GetComponents<Collider2D>())
                        boundary.enabled = true;
                    break;

                case GameEvents.EnterApartment:
                    print("ENTER APARTMENT");
                    // mainCamera.gameObject.transform.parent = null;
                    // StartCoroutine(SmoothCameraTransition(new Vector3(0, 3, -10)));
                    // mainCamera.gameObject.GetComponent<Zoom>().endSize = 9.28f;

                    // foreach (var transitionInElement in transitionInElements)
                    //     // StartCoroutine(FadeSprite(transitionInElement, true));
                    //     transitionInElement.SetActive(true);
                    //
                    // foreach (var transitionOutElement in transitionOutElements)
                    //     // StartCoroutine(FadeSprite(transitionOutElement, false));
                    //     transitionOutElement.SetActive(false);
                    apartmentsShade.SetActive(false);
                    hallwayShade.SetActive(true);

                    foreach (var hallway in hallways)
                        hallway.GetComponent<SpriteRenderer>().sortingOrder = 0;
                    foreach (var boundary in apartmentsBoundaries.GetComponents<Collider2D>())
                        boundary.enabled = true;
                    foreach (var boundary in hallwayBoundaries.GetComponents<Collider2D>())
                        boundary.enabled = false;
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
            var spriteRenderer = transitionalElement.GetComponent<SpriteRenderer>();
            // if (fadeIn) spriteRenderer.GetComponent<Collider2D>().enabled = false;

            var startColor = spriteRenderer.color;
            var blackTransparent = new Color(0, 0, 0, 0);
            var blackHalf = new Color(0, 0, 0, 0.5f);
            var endColor = fadeIn ? blackHalf : blackTransparent;
            var elapsedTime = 0f;

            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                var t = Mathf.Clamp01(elapsedTime / transitionDuration);
                spriteRenderer.color = Color.Lerp(startColor, endColor, t);
                yield return null;
            }

            // Ensure we end at exactly the target value
            spriteRenderer.color = endColor;
            // spriteRenderer.sortingOrder = fadeIn ? 7 : 3;

            yield return new WaitForSeconds(1);

            // if (!fadeIn)
            //     spriteRenderer.GetComponent<Collider2D>().enabled = true;
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