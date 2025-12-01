using System;
using System.Collections;
using UnityEngine;

namespace Dialog
{
    [Serializable]
    public class InGameActors
    {
        public ActorName actorName;
        public GameObject actor;
    }

    public class ZoomActorByLine : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private GameObject player;
        [SerializeField] private InGameActors[] actors;

        [Header("Dynamic Zoom Settings")] [SerializeField]
        private float maxZoom = 15f;

        [SerializeField] private float zoomSpeed = 2f;
        [SerializeField] private float resetDuration = 1f;
        [SerializeField] private AnimationCurve resetCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        private readonly float minZoom = 5f;
        private InGameActors _currentLineActor;

        private bool _enableDynamicZoom;
        private bool _isZooming;
        private Coroutine _resetCoroutine;
        private float _targetZoom;

        private void Start()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            if (mainCamera != null)
                _targetZoom = mainCamera.orthographicSize;
        }

        private void Update()
        {
            if (!_enableDynamicZoom || actors == null || actors.Length == 0)
                return;

            UpdateDynamicZoom();
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.LineNarrationStart)
            {
                var line = (NarrationDialogLine)eventData.Data;
                var actorName = line.actorName;
                var actor = Array.Find(actors, actor => actor.actorName == actorName);

                _currentLineActor = actor;
                _enableDynamicZoom = true;
            }

            if (eventData.Name == GameEvents.LineNarrationEnd)
            {
                var currentDialogProp = eventData.Data.GetType().GetProperty("_currentDialogue");
                if (currentDialogProp == null)
                    return;

                var currentDialog = (NarrationDialogLine)currentDialogProp.GetValue(eventData.Data);
                var nextDialogLine = currentDialog.nextDialogueLine;

                if (nextDialogLine == null && currentDialog.playerOptions.Length == 0)
                {
                    _enableDynamicZoom = false;

                    // Stop any existing reset coroutine
                    if (_resetCoroutine != null) StopCoroutine(_resetCoroutine);

                    // Start the smooth camera reset coroutine
                    _resetCoroutine = StartCoroutine(ResetCameraToCenter());
                }
            }
        }

        /// <summary>
        ///     Calculates the distance between player and actor at index 0 and adjusts camera zoom accordingly
        /// </summary>
        private void UpdateDynamicZoom()
        {
            var currentActor = _currentLineActor.actor;
            if (currentActor == null)
                return;

            // Calculate distance between player and first actor
            var distance = Vector3.Distance(
                player.transform.position,
                currentActor.transform.position
            );

            mainCamera.orthographicSize = Mathf.Lerp(
                mainCamera.orthographicSize,
                Mathf.Clamp(distance * 0.8f, minZoom, maxZoom),
                zoomSpeed * Time.deltaTime
            );

            // Position the camera between the player and the actor, but maintain original Z
            var midpoint = Vector3.Lerp(
                player.transform.position,
                currentActor.transform.position,
                0.5f
            );

            midpoint.z = mainCamera.transform.position.z;
            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position,
                midpoint,
                zoomSpeed * Time.deltaTime
            );
        }

        /// <summary>
        ///     Smoothly resets the camera position to center (0,0) over time
        /// </summary>
        /// <returns>Coroutine enumerator</returns>
        private IEnumerator ResetCameraToCenter()
        {
            if (mainCamera == null) yield break;

            var startPosition = mainCamera.transform.position;
            var targetPosition =
                new Vector3(
                    player.transform.position.x,
                    player.transform.position.y,
                    startPosition.z); // Keep original Z position
            var startZoom = mainCamera.orthographicSize;
            var targetZoom = _targetZoom; // Reset to original zoom level

            var elapsedTime = 0f;

            while (elapsedTime < resetDuration)
            {
                elapsedTime += Time.deltaTime;
                var progress = elapsedTime / resetDuration;
                var curveValue = resetCurve.Evaluate(progress);

                // Smoothly interpolate position
                mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, curveValue);

                // Smoothly interpolate zoom
                mainCamera.orthographicSize = Mathf.Lerp(startZoom, targetZoom, curveValue);

                yield return null;
            }

            // Ensure final position is exact
            mainCamera.transform.position = targetPosition;
            mainCamera.orthographicSize = targetZoom;

            _resetCoroutine = null;
        }
    }
}