using Dialog.Scripts;
using UnityEngine;

namespace Common.scripts
{
    public class LevelThoughtsController : ObserverSubject
    {
        [SerializeField] private ThoughtChoice knockingThoughts;

        public void OnNotify(GameEventData gameEvent)
        {
            if (gameEvent.Name == GameEvents.KnockOnPlayerApartment)
                Notify(GameEvents.AddThoughts, knockingThoughts);
        }
    }
}