using System.Collections;
using UnityEngine;

public class BreathAnimation : MonoBehaviour {
    [SerializeField] private float scaleMin = 0.95f;
    [SerializeField] private float scaleMax = 1.055f;
    [SerializeField] private float breathAmount = 0.01f;
    [SerializeField] private float waitBetweenBreathes = 2;
    private bool _breathIn = true;

    private void Start() {
        StartCoroutine(Breath());
    }

    private IEnumerator Breath() {
        while (true) {
            var newScale = _breathIn
                ? new Vector2(transform.localScale.x + breathAmount, transform.localScale.x + breathAmount)
                : new Vector2(transform.localScale.x - breathAmount, transform.localScale.x - breathAmount);

            transform.localScale = newScale;

            yield return new WaitForEndOfFrame();

            if (transform.localScale.x >= scaleMax || transform.localScale.x <= scaleMin) {
                yield return new WaitForSeconds(waitBetweenBreathes);
                _breathIn = !_breathIn;
            }
        }
    }
}