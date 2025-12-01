using System;
using common.scripts;
using Dialog;
using UnityEngine;

namespace Npc
{
    public class SwitchBodiesAndControl : MonoBehaviour
    {
        [SerializeField] private GameObject playerObject;
        [SerializeField] private GameObject playerSpriteObject;
        [SerializeField] private InGameActors[] actors;

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.PlayerControlNpc)
            {
                // Notify(action, new { _currentDialogue.actorName, _currentDialogue.actionNumberData });
                var actorNameProperty = eventData.Data.GetType().GetProperty("actorName");
                var npcName = actorNameProperty != null
                    ? (ActorName)actorNameProperty.GetValue(eventData.Data)
                    : ActorName.None;

                if (npcName == ActorName.None || actors == null || actors.Length == 0)
                    return;

                var npc = Array.Find(actors, actor => actor.actorName == npcName);
                if (npc == null)
                {
                    Debug.LogWarning($"Actor {npcName} not found in {nameof(SwitchBodiesAndControl)}");
                    return;
                }

                Debug.Log($"Found actor {npc.actorName} for control switch.");

                // Swap by deconstructing
                (playerObject.transform.position, npc.actor.gameObject.transform.position) =
                    (npc.actor.gameObject.transform.position, playerObject.transform.position);

                // Swap the actual sprites by setting the SpriteRenderer.sprite properties directly
                var playerSpriteRenderer = playerSpriteObject.GetComponent<SpriteRenderer>();
                var npcSpriteRenderer = npc.actor.gameObject.GetComponent<SpriteRenderer>();

                (playerSpriteRenderer.sprite, npcSpriteRenderer.sprite) =
                    (npcSpriteRenderer.sprite, playerSpriteRenderer.sprite);

                playerSpriteObject.GetComponent<Animator>().enabled = false;
                playerSpriteObject.GetComponent<AnimationController>().enabled = false;
            }
        }
    }
}