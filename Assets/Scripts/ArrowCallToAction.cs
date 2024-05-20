using System.Collections;
using UnityEngine;

public class ArrowCallToAction : MonoBehaviour {
    [SerializeField] private float speed = 1;
    [SerializeField] private float amount = 1;
    [SerializeField] private float direction = 1;
    private float _originalPosition;

    private void Start() {
        _originalPosition = transform.position.x;
        StartCoroutine(ArrowMovement());
    }

    private void OnMouseDown() {
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
    }

    private IEnumerator ArrowMovement() {
        while (true) {
            yield return new WaitForEndOfFrame();

            if (IsBeyondThreshold())
                direction *= -1;

            var translation = new Vector3(speed * direction * Time.deltaTime, 0) {
                z = transform.position.z
            };

            transform.Translate(translation);
        }
    }

    private bool IsBeyondThreshold() {
        return (direction > 0 && transform.position.x >= _originalPosition + amount) ||
               (direction < 0 && transform.position.x <= _originalPosition - amount);
    }
}