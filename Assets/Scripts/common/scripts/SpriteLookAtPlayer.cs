using UnityEngine;

public class SpriteLookAtPlayer : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        _spriteRenderer.flipX = transform.position.x < playerTransform.position.x;
    }
}
