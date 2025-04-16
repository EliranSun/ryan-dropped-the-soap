using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dialog.Scripts;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ActorReactionImage
{
    public Reaction reaction;
    public Sprite image;
}

public class DialogActorsFaceController : MonoBehaviour
{
    [SerializeField] Image leftSideImage;
    [SerializeField] Image rightSideImage;
    [SerializeField] GameObject leftSideContainer;
    [SerializeField] GameObject rightSideContainer;

    [SerializeField] private List<ActorReactionImage> stacyReactions;
    [SerializeField] private List<ActorReactionImage> oldManReactions;


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
            
            if (dialogLine.actorName == ActorName.Stacy)
            {
                var reaction = stacyReactions.First(stacy => stacy.reaction == dialogLine.actorReaction);
                rightSideImage.sprite = reaction.image;
                rightSideContainer.SetActive(true);
            }

            if (dialogLine.actorName == ActorName.OldMan)
            {
                var reaction = oldManReactions.First(stacy => stacy.reaction == dialogLine.actorReaction);
                leftSideImage.sprite = reaction.image; 
                leftSideContainer.SetActive(true);
            }
        }

        if (eventData.Name == GameEvents.LineNarrationEnd)
        {
            leftSideImage.sprite = null;
            rightSideImage.sprite = null;
            leftSideContainer.SetActive(false);
            rightSideContainer.SetActive(false);
        }
    }
}
