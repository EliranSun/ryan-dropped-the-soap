using Character.Scripts;
using UnityEngine;

public class StacySceneEvents : ObserverSubject
{
    [SerializeField] GameObject stacy;
    [SerializeField] Sprite stacyCrawlsSprite;
    [SerializeField] Sprite stacySleepsSprite;
    [SerializeField] Sprite stacyIdleSprite;
    [SerializeField] GameObject inWorldKnife;
    [SerializeField] int timeToWake = 5;

    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        if (stacy)
        {
            _spriteRenderer = stacy.GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = stacySleepsSprite;
            stacy.GetComponent<Movement>().enabled = false;

            Invoke(nameof(WakeUp), timeToWake);
        }
    }

    private void WakeUp()
    {
        _spriteRenderer.sprite = stacyIdleSprite;
        stacy.GetComponent<Movement>().enabled = true;
        Notify(GameEvents.ZoomStart);
    }

    public void OnNotify(GameEventData eventData)
    {
        if (eventData.Name == GameEvents.CrawlTrigger)
        {
            _spriteRenderer.sprite = stacyCrawlsSprite;
        }

        if (eventData.Name == GameEvents.IdleTrigger)
        {
            _spriteRenderer.sprite = stacyIdleSprite;
        }

        if (eventData.Name == GameEvents.RevealKnife)
            inWorldKnife.SetActive(true);
    }
}
