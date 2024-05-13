using System.Collections;
using UnityEngine;

public class BreathAnimation : MonoBehaviour {
    [SerializeField] private float scaleMin = 0.95f;
    [SerializeField] private float scaleMax = 1.055f;
    [SerializeField] private float growth = 1.01f;
    [SerializeField] private float rate = 0.1f;
    [SerializeField] private float waitBetween = 2;
    private bool _breathIn = true;

    private void Start() {
        StartCoroutine(Breath());
    }

    private IEnumerator Breath() {
        while (true) {
            if (_breathIn)
                transform.localScale *= growth;
            else
                transform.localScale /= growth;

            yield return new WaitForSeconds(rate);

            if (transform.localScale.x >= scaleMax || transform.localScale.x <= scaleMin) {
                yield return new WaitForSeconds(waitBetween);
                _breathIn = !_breathIn;
            }
        }
    }
}