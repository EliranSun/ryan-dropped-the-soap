using Character.Scripts;
using Dialog.Scripts;
using UnityEngine;

namespace Scenes.Stacy.scripts
{
    public class StacySceneEvents : ObserverSubject
    {
        [SerializeField] private GameObject stacy;
        [SerializeField] private Sprite stacyCrawlsSprite;
        [SerializeField] private Sprite stacySleepsSprite;
        [SerializeField] private Sprite stacyIdleSprite;
        [SerializeField] private GameObject inWorldKnife;
        [SerializeField] private NarrationDialogLine knifeRevealDialogLine;
        [SerializeField] private int timeToWake = 5;

        private SpriteRenderer _spriteRenderer;

        private void Start()
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
            if (eventData.Name == GameEvents.CrawlTrigger) _spriteRenderer.sprite = stacyCrawlsSprite;

            if (eventData.Name == GameEvents.IdleTrigger) _spriteRenderer.sprite = stacyIdleSprite;

            if (eventData.Name == GameEvents.RevealKnife)
            {
                inWorldKnife.SetActive(true);
                Invoke(nameof(TriggerKnifeRevealDialog), 4f);
            }
        }

        private void TriggerKnifeRevealDialog()
        {
            Notify(GameEvents.TriggerSpecificDialogLine, knifeRevealDialogLine);
        }
    }
}