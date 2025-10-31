using System;
using Dialog;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Mini_Games.Shout
{
    public class ShoutMiniGameController : MiniGame, IPointerClickHandler
    {
        [SerializeField] private Sprite[] avatars;
        [SerializeField] private Image[] avatarsImages;
        [SerializeField] private PlayerMiniGameChoice[] choices;

        private void Start()
        {
            for (var i = 0; i < avatarsImages.Length; i++)
                avatarsImages[i].sprite = avatars[i];

            // Notify(GameEvents.AddThoughts, new ThoughtChoice { choices = choices });
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            throw new NotImplementedException();
        }

        private void AddNpcThoughts()
        {
        }


        public override void OnNotify(GameEventData gameEventData)
        {
            if (gameEventData.Name is GameEvents.ThoughtDrop)
            {
            }
        }
    }
}