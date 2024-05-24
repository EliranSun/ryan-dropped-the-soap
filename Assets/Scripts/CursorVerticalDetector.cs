using System;
using UnityEngine;

public class CursorVerticalDetector : MonoBehaviour {
    [SerializeField] private int strongPullThreshold = 8;
    [SerializeField] private float gesturesMagnitudeThreshold = 0.5f; // Threshold of movement difference for a gestures
    [SerializeField] private float gesturesTimeThreshold = 2f; // Time threshold to reset gestures detection
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
            if (Time.time - _lastGestureTime < gesturesTimeThreshold)
                OnVerticalGesture(_lastPosition, mousePosition);

            _lastGestureTime = Time.time;
        }

        _lastPosition = mousePosition;
    }

    private void OnVerticalGesture(Vector3 lastPosition, Vector3 currentPosition) {
        var xChange = Math.Round(Mathf.Abs(lastPosition.x - currentPosition.x), 2);
        var yChange = Math.Round(Mathf.Abs(lastPosition.y - currentPosition.y), 2);
        var isBigUpwardsMotions = currentPosition.y - lastPosition.y > strongPullThreshold;
        var downwardsMotion = currentPosition.y < lastPosition.y;
        var isVertical = xChange < 1 && yChange > 0.5f;

        if (isBigUpwardsMotions)
            OnBigUpwardsMotion();

        if (isVertical)
            OnVerticalMotion(downwardsMotion);
    }

    protected virtual void OnBigUpwardsMotion() { }

    protected virtual void OnVerticalMotion(bool isDownwardMotion) { }
}