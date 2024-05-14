using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FlipSpriteEverySeconds : MonoBehaviour {
    [SerializeField] private bool isActive;
    [SerializeField] private int seconds;
    private SpriteRenderer _sprite;

    private void Start() {
        _sprite = GetComponent<SpriteRenderer>();
        StartCoroutine(Flip());
    }

    private IEnumerator Flip() {
        while (true) {
            if (!isActive)
                yield return new WaitUntil(() => isActive);

            yield return new WaitForSeconds(Random.Range(seconds - 1, seconds + 1));
            _sprite.flipX = !_sprite.flipX;
        }
    }
}