using System;
using Dialog.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace mini_games.scripts
{
    [Serializable]
    internal enum Emotion
    {
        Neutral,
        Happy,
        Excited,
        Disgusted
    }

    [Serializable]
    internal class SpriteEmotion
    {
        public Sprite sprite;
        public Emotion emotion;
    }

    [Serializable]
    internal class Thought
    {
        public PlayerChoice[] playerOptions;
    }

    public class FlirtMiniGame : ObserverSubject
    {
        [SerializeField] private Image characterImageContainer;
        [SerializeField] private SpriteRenderer backgroundRenderer;
        [SerializeField] private GameObject inGameTrigger;
        [SerializeField] private SpriteEmotion[] characterSpritesEmotion;

        private void Start()
        {
            Notify(GameEvents.AddThoughts, new Thought
            {
                playerOptions = new[]
                {
                    new PlayerChoice("HI"),
                    new PlayerChoice("BYE")
                }
            });
        }
    }
}