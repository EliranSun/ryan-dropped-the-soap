using UnityEngine;

namespace Mini_Games
{
    public class MiniGamesController : MonoBehaviour
    {
        [SerializeField] private GameObject lockPickMiniGameContainer;

        private void Start()
        {
            lockPickMiniGameContainer.SetActive(false);
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.MiniGameStart)
            {
                var miniGameName = (MiniGameName)eventData.Data;
                switch (miniGameName)
                {
                    case MiniGameName.LockPick:
                        lockPickMiniGameContainer.SetActive(true);
                        break;
                }
            }
        }
    }
}