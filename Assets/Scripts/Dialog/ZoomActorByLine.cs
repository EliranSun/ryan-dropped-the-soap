using System;
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
        private bool _isZooming;

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

            if (eventData.Name == GameEvents.LineNarrationEnd) enableDynamicZoom = false;
        }

        /// <summary>
        ///     Calculates the distance between player and actor at index 0 and adjusts camera zoom accordingly
        /// </summary>
        private void UpdateDynamicZoom()
        {
            if (actors[0]?.actor == null) return;

            // Calculate distance between player and first actor
            var distance = Vector3.Distance(player.transform.position, actors[0].actor.transform.position);

            // Calculate target zoom based on distance
            // Clamp distance to our min/max range for consistent zoom behavior
            var clampedDistance = Mathf.Clamp(distance, minDistance, maxDistance);

            // Map distance to zoom level (closer = more zoomed in, farther = more zoomed out)
            var normalizedDistance = (clampedDistance - minDistance) / (maxDistance - minDistance);
            print($"Normalized distance: {normalizedDistance} max: {maxZoom} min: {minZoom} max: {minDistance}");

            _targetZoom = Mathf.Lerp(maxZoom, minZoom, normalizedDistance);

            // Smoothly transition to target zoom
            if (Mathf.Abs(mainCamera.orthographicSize - _targetZoom) > 0.1f)
            {
                mainCamera.orthographicSize =
                    Mathf.Lerp(mainCamera.orthographicSize, _targetZoom, zoomSpeed * Time.deltaTime);
                _isZooming = true;
            }
            else
            {
                _isZooming = false;
            }
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
    }
}