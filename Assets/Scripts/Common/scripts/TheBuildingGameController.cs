using UnityEngine;

namespace Common.scripts
{
    public class TheBuildingGameController : ObserverSubject
    {
        [SerializeField] private GameObject mainTitle;
        [SerializeField] private GameObject player;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private bool skipTitle;

        [SerializeField] private int charlotteApartmentNumber = 104;
        [SerializeField] private int playerApartmentNumber = 603;

        private void Start()
        {
            if (skipTitle) StartGame();
            else Notify(GameEvents.DisablePlayerMovement);
        }

        public void StartGame()
        {
            mainTitle.SetActive(false);
            mainCamera.GetComponent<CameraPan>().SkipCameraPanAndEnablePlayer(player);
        }

        public void OnNotify(GameEventData eventData)
        {
            // TODO: Handling need to be more specific, this might be any transition
            if (eventData.Name == GameEvents.CameraTransitionEnded)
                Invoke(nameof(EnablePlayerControl), 1);
        }

        private void EnablePlayerControl()
        {
            Notify(GameEvents.EnablePlayerMovement);
            mainCamera.transform.parent = player.transform;
        }
    }
}