using System.Collections;
using UnityEngine;

public class WiggleObject : MonoBehaviour {
    [SerializeField] private float scaleMin = 0.95f;
    [SerializeField] private float scaleMax = 1.055f;
    // [SerializeField] private float growth = 1.01f;
    [SerializeField] private float rate = 0.1f;
    [SerializeField] private float waitBetween = 2;
    private bool _breathIn = true;

    private void Start() {
        StartCoroutine(Wiggle());
    }

    private IEnumerator Wiggle() {
        while (true) {
            var newRotation = transform.rotation;

            if (_breathIn) newRotation.z++;
            else newRotation.z--;

            transform.rotation = newRotation;

            yield return new WaitForSeconds(rate);

            if (transform.rotation.z >= scaleMax || transform.rotation.z <= scaleMin) {
                yield return new WaitForSeconds(waitBetween);
                _breathIn = !_breathIn;
            }
        }
    }
}