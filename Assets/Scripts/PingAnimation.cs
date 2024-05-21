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
    }

    private void Update() {
        if (transform.localScale.x <= 2f) {
            var growth = 1 * Time.deltaTime;
            var newScale = new Vector2(transform.localScale.x + growth, transform.localScale.y + growth);
            transform.localScale = newScale;

            var color = _spriteRenderer.color;
            color.a = Mathf.Clamp01(color.a - Time.deltaTime);
            _spriteRenderer.color = color;
        }
        else {
            transform.localScale = _originalScale;
            _spriteRenderer.color = _originalColor;
        }
    }
}