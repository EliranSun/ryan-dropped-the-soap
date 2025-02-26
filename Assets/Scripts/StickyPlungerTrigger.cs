using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class StickyPlungerTrigger : ObserverSubject
{
    [SerializeField] private Sprite defaultPlungerSprite;
    [SerializeField] private Sprite stickyPlungerSprite;
    [SerializeField] private int pullForce = 1;
    private bool _isSticky;
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // private void OnCollisionEnter2D(Collision2D other) {
    //     if (!other.gameObject.CompareTag("Plunger Sticky"))
    //         return;
    //
    //     EnableStickiness();
    // }
    //
    // private void OnCollisionExit2D(Collision2D other) {
    //     if (!other.gameObject.CompareTag("Plunger Sticky"))
    //         return;
    //
    //     DisableStickiness();
    // }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Plunger Sticky"))
            return;

        EnableStickiness();
    }

    //
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Plunger Sticky"))
            return;

        DisableStickiness();
    }

    private void EnableStickiness()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;

        _isSticky = true;
        Notify(GameEvents.TriggerStick);
    }

    private void DisableStickiness()
    {
        _isSticky = false;
        _rigidbody2D.constraints = RigidbodyConstraints2D.None;
        Notify(GameEvents.TriggerNonStick);
    }

    public void OnNotify(GameEventData eventData)
    {
        if (!_isSticky)
            return;

        if (eventData.Name == GameEvents.StrongPull)
        {
            print("STRONG PULL!");
            _spriteRenderer.sprite = defaultPlungerSprite;
            _rigidbody2D.AddForce(new Vector2(-1, 2) * pullForce);
            return;
        }

        if (eventData.Name == GameEvents.DownwardsControllerMotion)
            _spriteRenderer.sprite = defaultPlungerSprite;

        if (eventData.Name == GameEvents.UpwardsControllerMotion)
            _spriteRenderer.sprite = stickyPlungerSprite;
    }
}