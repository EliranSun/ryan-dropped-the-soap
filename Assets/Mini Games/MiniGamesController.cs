using UnityEngine;

namespace Mini_Games
{
    public class MiniGamesController : ObserverSubject
    {
        [SerializeField] private GameObject lockPickMiniGameContainer;

        private void Start()
        {
            // lockPickMiniGameContainer.SetActive(false);
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.TriggerLockPickMiniGame)
                Notify(GameEvents.StartMiniGames, MiniGameName.LockPick);
        }
    }
}