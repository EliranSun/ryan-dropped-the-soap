using System.Collections;
using Dialog.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace zeke
{
    public class ZekeShoutsController : ObserverSubject
    {
        [SerializeField] private Image zekeShoutsImage;
        [SerializeField] private Sprite[] zekeShoutsSprites;
        [SerializeField] private ThoughtChoice[] thoughtsChoice;
        [SerializeField] private ThoughtChoice finalChoice;
        private int _activeSpriteIndex;
        private bool _isInitialized;
        private int _shoutsCount;

        public void Init()
        {
            StartCoroutine(ThinkRandomThought());
            _isInitialized = true;
            zekeShoutsImage.sprite = zekeShoutsSprites[0];
        }

        public void OnNotify(GameEventData eventData)
        {
            if (
                eventData.Name != GameEvents.SpeakMind ||
                !_isInitialized ||
                _activeSpriteIndex >= zekeShoutsSprites.Length
            )
                return;

            _shoutsCount++;
            StartCoroutine(ThinkRandomThought());

            if (_shoutsCount % 4 == 0) // every X
            {
                _activeSpriteIndex++;

                if (_activeSpriteIndex >= zekeShoutsSprites.Length)
                {
                    // last
                    StopAllCoroutines();
                    Notify(GameEvents.KillThoughtsAndSayings);
                    _isInitialized = false;
                    zekeShoutsImage.sprite = zekeShoutsSprites[0];
                    Invoke(nameof(CloseSequence), 5f);
                    return;
                }

                zekeShoutsImage.sprite = zekeShoutsSprites[_activeSpriteIndex];
            }
        }

        private void CloseSequence()
        {
            Notify(GameEvents.AddThoughts, finalChoice);
            // Notify(GameEvents.EndZekeShouts);
        }

        private IEnumerator ThinkRandomThought()
        {
            while (true)
            {
                var randomIndex = Random.Range(0, thoughtsChoice.Length);
                var selectedThought = thoughtsChoice[randomIndex];

                Notify(GameEvents.AddThoughts, selectedThought);

                yield return new WaitForSeconds(1f);
            }
        }
    }
}