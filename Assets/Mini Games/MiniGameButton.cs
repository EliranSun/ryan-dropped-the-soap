using UnityEngine;

namespace Mini_Games
{
    public class MiniGameButton : ObserverSubject
    {
        [SerializeField] private MiniGameName miniGameName;

        public void OnClick()
        {
            Notify(GameEvents.StartMiniGames, miniGameName);
        }
    }
}