using System.Collections;
using common.scripts;
using Dialog;
using UnityEngine;
using UnityEngine.UI;

namespace Elevator.scripts
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(UICompoundMover))]
    public class SelectableKillableCharacter : ObserverSubject
    {
        [SerializeField] private Image bloodyScreen;
        [SerializeField] private Sprite deadImage;
        [SerializeField] private GameObject inWorldCharacter;
        [SerializeField] private Sprite inWorldDeadSprite;
        [SerializeField] private UICompoundMover uiCompoundMover;
        [SerializeField] private NarrationDialogLine[] lines;
        private int _attemptKillCount;
        private Image _image;

        private void Start()
        {
            _image = GetComponent<Image>();
            uiCompoundMover = GetComponent<UICompoundMover>();
        }

        public void OnNotify()
        {
            if (lines.Length > 0)
                Notify(GameEvents.TriggerSpecificDialogLine, lines[_attemptKillCount]);

            if (_attemptKillCount == lines.Length - 1 || lines.Length == 0)
            {
                ToggleBloodyScreen();
                bloodyScreen.GetComponent<TransitionController>().FadeInOut(3);
                Invoke(nameof(ToggleBloodyScreen), 6);
                Invoke(nameof(SwitchSprite), 1);
                Invoke(nameof(StartTransition), 5);
                // Do not increment _attemptKillCount further, as this is the last line.
                return;
            }

            _attemptKillCount++;
        }

        private void ToggleBloodyScreen()
        {
            bloodyScreen.gameObject.SetActive(!bloodyScreen.gameObject.activeSelf);
        }

        private void SwitchSprite()
        {
            _image.sprite = deadImage;

            var animator = inWorldCharacter.GetComponent<Animator>();
            if (animator) animator.enabled = false;

            inWorldCharacter.GetComponent<SpriteRenderer>().sprite = inWorldDeadSprite;
            Notify(GameEvents.MurderedNpc, gameObject.name);
        }

        private void StartTransition()
        {
            uiCompoundMover.enabled = false;
            StartCoroutine(TransitionSpriteDown());
        }

        private IEnumerator TransitionSpriteDown()
        {
            while (transform.position.y > -Screen.height)
            {
                transform.Translate(0, -5 * Time.deltaTime, 0);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}