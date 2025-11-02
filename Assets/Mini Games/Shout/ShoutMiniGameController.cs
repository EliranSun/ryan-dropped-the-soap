using System;
using System.Collections;
using Dialog;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Mini_Games.Shout
{
    public class ShoutMiniGameController : MiniGame, IPointerClickHandler
    {
        [SerializeField] private Sprite[] avatars;
        [SerializeField] private Image[] avatarsImages;
        [SerializeField] private PlayerMiniGameChoice[] choices;
        [SerializeField] private GameObject[] thoughtBlocks;

        private void Start()
        {
            for (var i = 0; i < avatarsImages.Length; i++)
                avatarsImages[i].sprite = avatars[i];

            StartCoroutine(AddRandomPlayerThought());
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            throw new NotImplementedException();
        }

        private IEnumerator AddRandomPlayerThought()
        {
            var waitTime = Random.Range(1f, 3f); // wait between 1 and 3 seconds between thoughts
            while (true)
            {
                // Pick a random choice from the array
                var randomIndex = Random.Range(0, choices.Length);
                var randomThought = choices[randomIndex];

                // Notify the system with the randomly selected thought as a single-item array
                Notify(GameEvents.AddThoughts, new ThoughtChoice { choices = new[] { randomThought } });

                // Wait for a random interval before adding another thought
                yield return new WaitForSeconds(waitTime);
                waitTime = Random.Range(1f, 3f);
            }
        }

        private void ShootNpcThoughts()
        {
        }

        private void LoadPlayerThought(string thoughtText, int thoughtScore)
        {
            switch (thoughtText.Length)
            {
                case < 10: Instantiate(thoughtBlocks[0]); break;
                case < 20: Instantiate(thoughtBlocks[1]); break;
                case < 30: Instantiate(thoughtBlocks[2]); break;
                case >= 30: Instantiate(thoughtBlocks[3]); break;
            }
        }

        private void ShootThought()
        {
        }


        public override void OnNotify(GameEventData gameEventData)
        {
            if (gameEventData.Name is GameEvents.ThoughtDrop)
            {
                var (thoughtText, thoughtScore) = ((string, int))gameEventData.Data;

                LoadPlayerThought(thoughtText, thoughtScore);
            }
        }
    }
}