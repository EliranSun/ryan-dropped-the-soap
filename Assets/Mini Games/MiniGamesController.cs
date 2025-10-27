using UnityEngine;

namespace Mini_Games
{
    public class MiniGamesController : ObserverSubject
    {
        [SerializeField] private GameObject lockPickMiniGameContainer;


        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.TriggerLockPickMiniGame)
                Notify(GameEvents.StartMiniGames, MiniGameName.LockPick);
        }
    }
}