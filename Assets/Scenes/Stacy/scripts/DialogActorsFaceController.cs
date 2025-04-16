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

    [SerializeField] private List<ActorReactionImage> stacyReactions;

    public void OnNotify(GameEventData eventData)
    {
        if (eventData.Name == GameEvents.ActorReaction)
        {
            var dialogLine = eventData.Data as NarrationDialogLine;
            if (dialogLine.actorName == ActorName.Stacy)
            {
                var reaction = stacyReactions.First(stacy => stacy.reaction == dialogLine.actorReaction);
                rightSideImage.sprite = reaction.image;
            }
            else
            {
                rightSideImage.sprite = null;
            }
        }
    }
}
