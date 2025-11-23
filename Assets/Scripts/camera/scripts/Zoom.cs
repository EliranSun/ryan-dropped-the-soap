using System;
using Elevator.scripts;
using Object.Scripts;
using Player;
using UnityEngine;

namespace Camera.scripts
{
    public class Zoom : ObserverSubject
    {
        private const float Tolerance = 0.01f;

        [Header("Settings")] [SerializeField] private float delay;

        [SerializeField] private float speed = 1;
        [SerializeField] public float startSize = 1;
        [SerializeField] public float endSize = 6;
        [SerializeField] private bool isActiveByTrigger;
        [SerializeField] private Transform target;

        [Header("Layers")] [SerializeField] private float outsideZoom = 25;

        [SerializeField] private float insideBuildingZoom = 18;
        [SerializeField] private float insideElevatorZoom = 14;
        [SerializeField] private float insideStaircaseZoom = 10;

        private Vector3 _initialCameraPosition;
        private bool _isActive;
        private UnityEngine.Camera _mainCamera;
        private Vector3 _targetPosition;
        private float _time;
        private float _zoomProgress;

        public static Zoom Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
        }

        private void Start()
        {
            _time = Time.time;
            _mainCamera = UnityEngine.Camera.main;

            if (_mainCamera != null)
            {
                _mainCamera.orthographicSize = startSize;
                _initialCameraPosition = _mainCamera.transform.position;
            }

            if (IsZoomReached()) Notify(GameEvents.ZoomChangeEnd);
        }

        private void Update()
        {
            _time = Time.time;

            if (delay > 0 && _time < delay) return;

            if (isActiveByTrigger && !_isActive) return;

            // Check if zoom is complete
            if (IsZoomReached() ||
                Vector3.Distance(_mainCamera.transform.position, _targetPosition) < Tolerance)
            {
                Notify(GameEvents.ZoomChangeEnd);
                return;
            }

            // Calculate zoom progress (0 to 1)
            var sizeRange = Mathf.Abs(startSize - endSize);
            var currentProgress = Mathf.Abs(startSize - _mainCamera.orthographicSize) / sizeRange;
            _zoomProgress = Mathf.Clamp01(currentProgress);


            if (_mainCamera.orthographicSize > endSize)
                _mainCamera.orthographicSize -= Time.deltaTime * speed;
            else
                _mainCamera.orthographicSize += Time.deltaTime * speed;

            // Update camera position if target exists
            if (target != null)
            {
                // Store target position (keeping the current z value of the camera)
                _targetPosition = new Vector3(target.position.x, target.position.y, _mainCamera.transform.position.z);

                // Lerp camera position from initial to target based on zoom progress
                _mainCamera.transform.position = Vector3.Lerp(_initialCameraPosition, _targetPosition, _zoomProgress);
            }
        }

        private void SetZoomBasedOnLayer(BuildingLayerType layerName)
        {
            switch (layerName)
            {
                case BuildingLayerType.Hallway:
                    endSize = insideBuildingZoom;
                    break;

                case BuildingLayerType.Outside:
                    endSize = outsideZoom;
                    break;

                case BuildingLayerType.Staircase:
                    endSize = insideStaircaseZoom;
                    break;

                case BuildingLayerType.Elevator:
                    endSize = insideElevatorZoom;
                    break;
            }
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.LayerChange)
                SetZoomBasedOnLayer((BuildingLayerType)eventData.Data);

            if (eventData.Name == GameEvents.ZoomStart)
            {
                _isActive = true;
                _zoomProgress = 0f;
                if (_mainCamera != null) _initialCameraPosition = _mainCamera.transform.position;
            }

            if (eventData.Name == GameEvents.PlayerInteraction)
            {
                var interactedObject = (Interaction)eventData.Data;

                switch (interactedObject.objectName)
                {
                    case ObjectNames.BuildingExit:
                        SetZoomBasedOnLayer(BuildingLayerType.Outside);
                        break;

                    case ObjectNames.StaircaseEntrance:
                        SetZoomBasedOnLayer(BuildingLayerType.Staircase);
                        break;

                    case ObjectNames.ElevatorEnterDoors:
                        SetZoomBasedOnLayer(BuildingLayerType.Elevator);
                        break;

                    case ObjectNames.StaircaseExit:
                    case ObjectNames.BuildingEntrance:
                    case ObjectNames.ElevatorExitDoors:
                        SetZoomBasedOnLayer(BuildingLayerType.Hallway);
                        break;
                }
            }
        }

        public void ResetZoom()
        {
            if (_mainCamera != null)
            {
                _mainCamera.orthographicSize = startSize;
                _mainCamera.transform.position = _initialCameraPosition;
                _zoomProgress = 0f;
            }
        }

        private bool IsZoomReached()
        {
            return Math.Abs(_mainCamera.orthographicSize - endSize) < Tolerance;
        }
    }
}