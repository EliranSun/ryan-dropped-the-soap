using System.Collections;
using common.scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Elevator.scripts
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(UICompoundMover))]
    public class SelectableKillableCharacter : MonoBehaviour
    {
        [SerializeField] private Image bloodyScreen;
        [SerializeField] private Sprite deadImage;
        [SerializeField] private UICompoundMover uiCompoundMover;
        private Image _image;

        private void Start()
        {
            _image = GetComponent<Image>();
            uiCompoundMover = GetComponent<UICompoundMover>();
        }

        public void OnNotify()
        {
            bloodyScreen.gameObject.SetActive(true);
            bloodyScreen.GetComponent<TransitionController>().FadeInOut(3);
            Invoke(nameof(SwitchSprite), 1);
            Invoke(nameof(StartTransition), 5);
        }

        private void SwitchSprite()
        {
            _image.sprite = deadImage;
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