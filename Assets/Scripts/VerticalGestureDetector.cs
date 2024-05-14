using System;
using UnityEngine;

public class VerticalGestureDetector : ObserverSubject {
    [SerializeField] private float gesturesMagnitudeThreshold = 0.5f; // Threshold of movement difference for a gestures
    [SerializeField] private float gesturesTimeThreshold = 1f; // Time threshold to reset gestures detection
    private int _gesturesCount;
    private float _lastGestureTime;
    private Vector3 _lastPosition;
    private Camera _mainCamera;

    private void Start() {
        _mainCamera = Camera.main;
        _lastGestureTime = Time.time;

        if (_mainCamera)
            _lastPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseDrag() {
        var mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var gestureMagnitude = (mousePosition - _lastPosition).magnitude;


        if (gestureMagnitude > gesturesMagnitudeThreshold) {
            var isVerticalGesture = IsVerticalGesture(_lastPosition, mousePosition);

            if (Time.time - _lastGestureTime < gesturesTimeThreshold)
                if (isVerticalGesture)
                    _gesturesCount++;

            _lastGestureTime = Time.time;
        }

        _lastPosition = mousePosition;

        // Optionally, you can reset the gestures count after some inactivity
        if (Time.time - _lastGestureTime > gesturesTimeThreshold && _gesturesCount > 0) _gesturesCount = 0;
    }

    private bool IsVerticalGesture(Vector3 lastPosition, Vector3 currentPosition) {
        var xChange = Math.Round(Mathf.Abs(lastPosition.x - currentPosition.x), 2);
        var yChange = Math.Round(Mathf.Abs(lastPosition.y - currentPosition.y), 2);
        var isBigUpwardsMotions = currentPosition.y - lastPosition.y > 2;
        var downwardsMotion = currentPosition.y < lastPosition.y;
        print($"{yChange}");
        var isVertical = xChange < 1 && yChange > 0.5f;

        if (isBigUpwardsMotions) {
            Notify(GameEvents.StrongPull);
            Notify(GameEvents.TriggerNonStick);
        }

        if (isVertical)
            Notify(downwardsMotion
                ? GameEvents.DownwardsControllerMotion
                : GameEvents.UpwardsControllerMotion);

        return isVertical;
    }
}