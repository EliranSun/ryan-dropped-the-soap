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
        [SerializeField] private Camera mainCamera;
        [SerializeField] private FloorData floorData;
        [SerializeField] private NarrationDialogLine breakIntoCharlotteApartmentLine;
        [SerializeField] private ElevatorController elevatorController;
        [SerializeField] private BuildingLayerType playerCurrentLayer;
        [SerializeField] private int playerCurrentFloor;
        [SerializeField] private bool skipTitle;
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
        }

        private void EnablePlayerControl()
        {
            Notify(GameEvents.EnablePlayerMovement);
            mainCamera.transform.parent = player.transform;
        }
    }
}