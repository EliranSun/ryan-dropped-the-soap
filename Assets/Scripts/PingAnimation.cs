using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PingAnimation : MonoBehaviour {
    private Color _originalColor;
    private Vector2 _originalScale;
    private SpriteRenderer _spriteRenderer;

    private void Start() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalScale = transform.localScale;
        _originalColor = _spriteRenderer.color;

        StartCoroutine(Ping());
    }

    private IEnumerator Ping() {
        while (_spriteRenderer.color.a > 0 || transform.localScale.x <= _originalScale.x * 1.5f) {
            yield return new WaitForEndOfFrame();
            var growth = 1 * Time.deltaTime;
            var newScale = new Vector2(transform.localScale.x + growth, transform.localScale.y + growth);
            transform.localScale = newScale;

            // var color = _spriteRenderer.color;
            // color.a = Mathf.Clamp01(color.a - 0.1f);
            // _spriteRenderer.color = color;
        }


        yield return new WaitForSeconds(2);

        transform.localScale = _originalScale;
        // _spriteRenderer.color = _originalColor;
        //
        // StartCoroutine(Ping());
    }
}