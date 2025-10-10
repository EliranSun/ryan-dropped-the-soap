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
        private float minZoom = 5f;

        [SerializeField] private float maxZoom = 15f;
        [SerializeField] private float zoomSpeed = 2f;
        [SerializeField] private float minDistance = 2f;
        [SerializeField] private float maxDistance = 10f;
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
            var targetPos = Vector3.Lerp(player.transform.position, actors[0].actor.transform.position, 0.5f);
            targetPos.z = mainCamera.transform.position.z;
            mainCamera.transform.position = targetPos;
        }

        /// <summary>
        ///     Manually set the zoom level (useful for cutscenes or specific dialog moments)
        /// </summary>
        /// <param name="zoomLevel">The desired orthographic size</param>
        /// <param name="instant">Whether to apply zoom instantly or smoothly</param>
        public void SetZoom(float zoomLevel, bool instant = false)
        {
            _targetZoom = Mathf.Clamp(zoomLevel, minZoom, maxZoom);

            if (instant) mainCamera.orthographicSize = _targetZoom;
        }

        /// <summary>
        ///     Enable or disable dynamic zoom
        /// </summary>
        /// <param name="isEnabled">Whether dynamic zoom should be active</param>
        public void SetDynamicZoomEnabled(bool isEnabled)
        {
            enableDynamicZoom = isEnabled;
        }

        /// <summary>
        ///     Get the current distance between player and actor at index 0
        /// </summary>
        /// <returns>Distance in world units</returns>
        public float GetPlayerActorDistance()
        {
            if (actors == null || actors.Length == 0 || actors[0]?.actor == null || player == null)
                return 0f;

            return Vector3.Distance(player.transform.position, actors[0].actor.transform.position);
        }

        /// <summary>
        ///     Check if the camera is currently zooming
        /// </summary>
        /// <returns>True if camera is transitioning to a new zoom level</returns>
        public bool IsZooming()
        {
            return _isZooming;
        }

        /// <summary>
        ///     Smoothly resets the camera position to center (0,0) over time
        /// </summary>
        /// <returns>Coroutine enumerator</returns>
        private IEnumerator ResetCameraToCenter()
        {
            if (mainCamera == null) yield break;

            var startPosition = mainCamera.transform.position;
            var targetPosition = new Vector3(0, 0, startPosition.z); // Keep original Z position
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