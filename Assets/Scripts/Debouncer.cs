using System;
using System.Collections;
using UnityEngine;

public class Debouncer : MonoBehaviour {
    private Action action;
    private Coroutine debounceCoroutine;
    private float delayInSeconds;

    public void Setup(float _delayInSeconds, Action _action) {
        delayInSeconds = _delayInSeconds;
        action = _action;
    }

    public void Debounce() {
        if (debounceCoroutine != null) StopCoroutine(debounceCoroutine);
        debounceCoroutine = StartCoroutine(DebounceRoutine());
    }

    private IEnumerator DebounceRoutine() {
        yield return new WaitForSeconds(delayInSeconds);
        action?.Invoke();
    }
}