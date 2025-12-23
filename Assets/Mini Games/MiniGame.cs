using Dialog;
using UnityEngine;

namespace Mini_Games
{
    public class MiniGame : ObserverSubject
    {
        [Header("Mini Game Settings")] [SerializeField]
        public MiniGameName miniGameName;

        [SerializeField] public string instructions;

        [SerializeField] public GameObject miniGameContainer;
        // [SerializeField] public GameObject inGameTrigger;

        protected virtual void StartMiniGame()
        {
            if (miniGameContainer)
                miniGameContainer.SetActive(true);
        }

        protected virtual void CloseMiniGame(bool isGameWon = false)
        {
            if (miniGameContainer)
                miniGameContainer.SetActive(false);

            Notify(isGameWon ? GameEvents.MiniGameWon : GameEvents.MiniGameLost, isGameWon ? 1 : 0);

            Invoke(nameof(MiniGameCleanups), 4);
        }

        private void MiniGameCleanups()
        {
            Notify(GameEvents.KillThoughtsAndSayings);
            Notify(GameEvents.KillDialog);
            Notify(GameEvents.MiniGameClosed);
        }


        public virtual void OnNotify(GameEventData eventData)
        {
            switch (eventData.Name)
            {
                case GameEvents.StartMiniGames:
                    var eventMiniGameName = (MiniGameName)eventData.Data;
                    if (eventMiniGameName == miniGameName) StartMiniGame();
                    break;

                case GameEvents.ClickOnNpc:
                    var interactionData = eventData.Data as InteractionData;
                    if (interactionData == null) return;

                    // if (interactionData.Name == inGameTrigger.gameObject.name)
                    //     StartMiniGame();
                    break;
            }
        }
    }
}