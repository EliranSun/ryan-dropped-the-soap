using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Dialog
{
    [Serializable]
    internal class InGameActors
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
        [SerializeField] private bool enableDynamicZoom;
        [SerializeField] private float resetDuration = 1f;
        [SerializeField] private AnimationCurve resetCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

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
            if (!enableDynamicZoom || actors == null || actors.Length == 0 || player == null)
                return;

            UpdateDynamicZoom();
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.LineNarrationStart)
            {
                var line = (NarrationDialogLine)eventData.Data;
                var actorName = line.actorName;
                if (actors.Any(actor => actor.actorName == actorName)) enableDynamicZoom = true;
            }

            if (eventData.Name == GameEvents.LineNarrationEnd)
            {
                var currentDialogProp = eventData.Data.GetType().GetProperty("_currentDialogue");
                if (currentDialogProp == null)
                    return;

                var currentDialog = (NarrationDialogLine)currentDialogProp.GetValue(eventData.Data);
                var nextDialogLine = currentDialog.nextDialogueLine;
                if (nextDialogLine == null)
                {
                    enableDynamicZoom = false;

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
            if (actors[0]?.actor == null)
                return;

            // Calculate distance between player and first actor
            var distance = Vector3.Distance(player.transform.position, actors[0].actor.transform.position);

            mainCamera.orthographicSize =
                Mathf.Lerp(mainCamera.orthographicSize, distance, zoomSpeed * Time.deltaTime);

            // Position the camera between the player and the actor, but maintain original Z
            var targetPos = Vector3.Lerp(mainCamera.transform.position, actors[0].actor.transform.position, 0.5f);
            targetPos.z = mainCamera.transform.position.z;
            mainCamera.transform.position = targetPos;
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