using Common.scripts;
using Dialog;
using UnityEngine;

namespace Elevator.scripts
{
    public class TheBuildingGameController : ObserverSubject
    {
        [SerializeField] private GameObject mainTitle;
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject charlotte;
        [SerializeField] private GameObject ryan;
        [SerializeField] private GameObject gun;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private FloorData floorData;
        [SerializeField] private NarrationDialogLine breakIntoCharlotteApartmentLine;
        [SerializeField] private NarrationDialogLine postLockPickMiniGameLine;
        [SerializeField] private ElevatorController elevatorController;
        [SerializeField] private BuildingLayerType playerCurrentLayer;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioSource soundEffectAudioSource;
        [SerializeField] private AudioClip ryanThemeSong;
        [SerializeField] private AudioClip unlockApartmentSound;
        [SerializeField] private int playerCurrentFloor;
        [SerializeField] private bool skipTitle;
        [SerializeField] private float ryanPostMiniGamePosition = 21.3f;
        private bool _brokeIntoCharlotteApartment;

        private void Start()
        {
            if (skipTitle) StartGame();
            else Notify(GameEvents.DisablePlayerMovement);
            Notify(GameEvents.ChangeActiveLayer, playerCurrentLayer);

            if (playerCurrentLayer == BuildingLayerType.InBuilding && playerCurrentFloor != 0)
            {
                var y = (playerCurrentFloor - 1) * elevatorController.floorHeight + 5;
                player.transform.position = new Vector3(0, y, player.transform.position.z);
            }
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

            if (eventData.Name == GameEvents.KnockOnNpcDoor)
            {
                var doorInfo = (DoorInfo)eventData.Data;
                if (doorInfo.residentName == ActorName.Charlotte && !_brokeIntoCharlotteApartment)
                {
                    // TODO: Position at door, so need to get the door pos here
                    charlotte.transform.position = doorInfo.door.transform.position;
                    Notify(GameEvents.TriggerSpecificDialogLine, breakIntoCharlotteApartmentLine);
                }
            }

            if (eventData.Name is GameEvents.MiniGameWon or GameEvents.MiniGameLost)
            {
                ryan.SetActive(true);
                var newPosition = new Vector3(
                    ryanPostMiniGamePosition,
                    player.transform.position.y,
                    ryan.transform.position.z
                );
                ryan.transform.position = newPosition;

                Invoke(nameof(InvokeRyanThemeSong), 5f);
                Notify(GameEvents.TriggerSpecificDialogLine, postLockPickMiniGameLine);
            }

            if (eventData.Name == GameEvents.RyanPullGun)
            {
                gun.SetActive(true);
                gun.transform.parent = ryan.transform;
                gun.transform.localPosition = new Vector2(0.666f, 0.8f);
            }

            if (eventData.Name == GameEvents.UnlockRyanApartment)
                if (unlockApartmentSound && soundEffectAudioSource)
                    soundEffectAudioSource.PlayOneShot(unlockApartmentSound);

            if (eventData.Name == GameEvents.RyanGivePlayerGun)
            {
                gun.SetActive(true);
                gun.transform.parent = player.transform;
                gun.transform.localPosition = new Vector2(0.666f, 0.8f);
            }
        }

        private void InvokeRyanThemeSong()
        {
            audioSource.clip = ryanThemeSong;
            audioSource.Play();
        }

        private void EnablePlayerControl()
        {
            Notify(GameEvents.EnablePlayerMovement);
            mainCamera.transform.parent = player.transform;
        }
    }
}