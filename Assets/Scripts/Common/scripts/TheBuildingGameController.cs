using UnityEngine;

namespace Common.scripts
{
    public class TheBuildingGameController : ObserverSubject
    {
        [SerializeField] private GameObject mainTitle;
        [SerializeField] private GameObject player;
        [SerializeField] private Camera mainCamera;

        private void Start()
        {
            Notify(GameEvents.DisablePlayerMovement);
        }

        public void StartGame()
        {
            mainTitle.SetActive(false);
            mainCamera.GetComponent<CameraPan>().GoToGroundFloor();
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