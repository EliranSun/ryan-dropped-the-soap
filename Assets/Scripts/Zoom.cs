using System;
using UnityEngine;

public class Zoom : MonoBehaviour
{
    private const float Tolerance = 0.01f;
    [SerializeField] private float delay;
    [SerializeField] private float speed = 1;
    [SerializeField] private float startSize = 1;
    [SerializeField] public float endSize = 6;
    private Camera _mainCamera;
    private float _time;

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
        _mainCamera = Camera.main;

        if (_mainCamera != null)
            _mainCamera.orthographicSize = startSize;
    }

    private void Update()
    {
        _time = Time.time;

        if (delay > 0 && _time < delay)
            return;

        if (Math.Abs(_mainCamera.orthographicSize - endSize) < Tolerance)
            return;

        if (_mainCamera.orthographicSize > endSize)
            _mainCamera.orthographicSize -= Time.deltaTime * speed;
        else
            _mainCamera.orthographicSize += Time.deltaTime * speed;
    }
}