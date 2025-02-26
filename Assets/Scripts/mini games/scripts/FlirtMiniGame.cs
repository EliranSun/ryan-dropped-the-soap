using System;
using System.Linq;
using Dialog.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace mini_games.scripts
{
    [Serializable]
    internal class SpriteEmotion
    {
        public Sprite sprite;
        public Reaction reaction;
    }

    [Serializable]
    public class FlirtChoice
    {
        public string text;
        public int score;
        public NarrationDialogLine actorLine;
    }

    public class FlirtMiniGame : ObserverSubject
    {
        public int score = 50;
        [SerializeField] private ActorName actorName = ActorName.Morgan;
        [SerializeField] private FlirtChoice[] choices;
        [SerializeField] private SpriteEmotion[] actorSpriteEmotions;

        [SerializeField] private Image characterImageContainer;
        [SerializeField] private SpriteRenderer backgroundRenderer;
        [SerializeField] private GameObject inGameTrigger;


        private void Start()
        {
            // Notify(GameEvents.AddThoughts, new ThoughtChoice { playerOptions = choices });
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.ActorReaction)
            {
                var dialogLine = eventData.Data as NarrationDialogLine;
                if (!dialogLine) return;
                if (dialogLine.actorName != actorName) return;

                var reaction = actorSpriteEmotions.First(x => x.reaction == dialogLine.actorReaction);
                characterImageContainer.sprite = reaction.sprite;
            }
        }

        public void SetActorName(ActorName actorName)
        {
            this.actorName = actorName;
        }
    }
}