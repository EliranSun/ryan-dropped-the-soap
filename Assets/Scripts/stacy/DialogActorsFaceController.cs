using System;
using System.Collections.Generic;
using System.Linq;
using Dialog;
using Expressions;
using UnityEngine;
using UnityEngine.UI;

namespace stacy
{
    [Serializable]
    public class ActorReactionImage
    {
        public Expression reaction;
        public Sprite image;
    }

    [Serializable]
    public class ActorConfig
    {
        public ActorName actorName;
        public List<ActorReactionImage> reactions;
        public bool isLeftSide;
    }

    public class DialogActorsFaceController : MonoBehaviour
    {
        [SerializeField] private Image leftSideImage;
        [SerializeField] private Image rightSideImage;
        [SerializeField] private GameObject leftSideContainer;
        [SerializeField] private GameObject rightSideContainer;

        [SerializeField] private List<ActorConfig> actorConfigs;
        [SerializeField] private Sprite charlotteBody;


        private void Start()
        {
            leftSideContainer.SetActive(false);
            rightSideContainer.SetActive(false);
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.ActorReaction)
            {
                var dialogLine = eventData.Data as NarrationDialogLine;

                if (dialogLine == null) return;

                if (dialogLine.actorName == ActorName.None)
                {
                    ClearAllActors();
                    return;
                }

                SetActorReaction(dialogLine.actorName, dialogLine.actorReaction);
            }

            if (eventData.Name == GameEvents.LineNarrationEnd)
            {
                ClearAllActors();
            }
        }

        private void SetActorReaction(ActorName actorName, Expression reaction)
        {
            var actorConfig = actorConfigs.FirstOrDefault(config => config.actorName == actorName);
            if (actorConfig == null) return;

            var reactionImage = actorConfig.reactions.FirstOrDefault(r => r.reaction == reaction);
            if (reactionImage == null) return;

            if (actorConfig.isLeftSide)
            {
                leftSideImage.sprite = reactionImage.image;
                // Resize the image to match the sprite dimensions
                if (reactionImage.image != null)
                {
                    leftSideImage.rectTransform.sizeDelta = new Vector2(reactionImage.image.rect.width, reactionImage.image.rect.height);
                }
                leftSideContainer.SetActive(true);
            }
            else
            {
                rightSideImage.sprite = reactionImage.image;
                // Resize the image to match the sprite dimensions
                if (reactionImage.image != null)
                {
                    rightSideImage.rectTransform.sizeDelta = new Vector2(reactionImage.image.rect.width, reactionImage.image.rect.height);
                }
                rightSideContainer.SetActive(true);
            }
        }

        private void ClearAllActors()
        {
            leftSideImage.sprite = null;
            rightSideImage.sprite = null;
            leftSideContainer.SetActive(false);
            rightSideContainer.SetActive(false);
        }
    }
}