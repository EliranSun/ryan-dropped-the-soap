using Character.Scripts;
using common.scripts;
using Dialog;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Scenes.Stacy.scripts
{
    public class StacySceneEvents : ObserverSubject
    {
        [SerializeField] private GameObject stacy;
        [SerializeField] private Sprite stacyCrawlsSprite;
        [SerializeField] private Sprite stacySleepsSprite;
        [SerializeField] private Sprite stacyIdleSprite;
        [SerializeField] private GameObject inWorldKnife;
        [SerializeField] private GameObject inWorldRope;
        [SerializeField] private GameObject flashlight;
        [SerializeField] private GameObject oldMan;
        [SerializeField] private GameObject cabin;
        [SerializeField] private GameObject stacyMonologue;
        [SerializeField] private GameObject moon;
        [SerializeField] private int timeToWake = 5;
        [SerializeField] private GameObject sceneLight;
        [SerializeField] private NarrationDialogLine knifeRevealDialogLine;
        [SerializeField] private NarrationDialogLine treasureFoundDialogLine;
        [SerializeField] private NarrationDialogLine walkInCabinDialogLine;
        [SerializeField] private Material litMaterial;
        [SerializeField] private Material unlitMaterial;
        private int _bodyCount;

        private SpriteRenderer _spriteRenderer;
        private bool _treasureFound;

        private void Start()
        {
            if (treasureFoundDialogLine.lineCondition.isMet) _treasureFound = true;
            inWorldRope.GetComponent<SimpleInteraction>().isEnabled = false;

            stacyMonologue.SetActive(false);

            if (stacy)
            {
                _spriteRenderer = stacy.GetComponent<SpriteRenderer>();
                _spriteRenderer.sprite = stacySleepsSprite;
                stacy.GetComponent<Movement>().enabled = false;

                Invoke(nameof(WakeUp), timeToWake);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z)) stacy.transform.position = new Vector3(64, 9, 0);
            if (Input.GetKeyDown(KeyCode.O)) stacy.transform.position = new Vector3(-64, 45, 0);
            if (Input.GetKeyDown(KeyCode.H)) stacy.transform.position = new Vector3(0, -1, 0);
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
                _spriteRenderer.sprite = stacyCrawlsSprite;

            if (eventData.Name == GameEvents.IdleTrigger)
                _spriteRenderer.sprite = stacyIdleSprite;

            if (eventData.Name == GameEvents.RevealKnife)
            {
                inWorldKnife.SetActive(true);
                Invoke(nameof(TriggerKnifeRevealDialog), 4f);
            }

            if (eventData.Name == GameEvents.NpcDeath)
                _bodyCount++;

            if (eventData.Name == GameEvents.EnableRope)
                inWorldRope.GetComponent<SimpleInteraction>().isEnabled = true;

            if (eventData.Name == GameEvents.TreasureFound)
            {
                _treasureFound = true;

                sceneLight.GetComponent<Light2D>().intensity = 0.02f;
                flashlight.GetComponent<ToggleLight>().DisableLightToggle();

                stacyMonologue.SetActive(true);

                oldMan.transform.position = cabin.transform.position;

                Invoke(nameof(TriggerTreasureFoundDialog), 3f);
            }

            if (eventData.Name == GameEvents.GlobalLightToggle)
            {
                var isDark = Mathf.Approximately(sceneLight.GetComponent<Light2D>().intensity, 0.02f);
                sceneLight.GetComponent<Light2D>().intensity = isDark ? 0.8f : 0.02f;
                moon.SetActive(isDark);
                stacy.GetComponent<SpriteRenderer>().material = isDark ? unlitMaterial : litMaterial;
            }

            if (eventData.Name == GameEvents.NpcDeath)
            {
                var isOldManDead = eventData.Data as string == oldMan.gameObject.name;
                print("Is old man dead? " + isOldManDead + "; is treasure found? " + _treasureFound);
                if (isOldManDead && _treasureFound)
                    walkInCabinDialogLine.lineCondition.isMet = true;
            }
        }

        private void TriggerKnifeRevealDialog()
        {
            Notify(GameEvents.TriggerSpecificDialogLine, knifeRevealDialogLine);
        }

        private void TriggerTreasureFoundDialog()
        {
            Notify(GameEvents.TriggerSpecificDialogLine, knifeRevealDialogLine);
        }
    }
}