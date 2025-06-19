using System;
using System.Linq;
using common.scripts;
using Common.scripts;
using Player;
using TMPro;
using UnityEngine;

namespace Elevator.scripts
{
    [RequireComponent(typeof(Collider2D))]
    public class DoorController : ObserverSubject
    {
        [SerializeField] private TextMeshPro doorNumberTextMeshPro;
        [SerializeField] private TransitionController transitionImage;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private GameObject[] doors;
        [SerializeField] private FloorData floorData;
        [SerializeField] private AudioClip knockSound;
        [SerializeField] private int doorNumber;
        [SerializeField] private GameObject npcAtDoor;

        private bool _isDoorOpen;
        private bool _isPlayerInsideApartment = true;
        private bool _isPlayerOnDoor;
        private GameObject[] _objectsInsideApartment = { };

        // Update is called once per frame
        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.X))
                return;

            if (_isPlayerOnDoor && _isDoorOpen)
                Notify(GameEvents.ChangePlayerLocation, Location.Hallway);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                _isPlayerOnDoor = true;

            if (other.CompareTag("NPC"))
                MoveObjectToLinkedDoor(other.transform);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                _isPlayerOnDoor = false;
        }

        private void HandlePlayerApartmentDoorClick()
        {
            _isDoorOpen = !_isDoorOpen;

            foreach (var door in doors)
            {
                // door.SetActive(!_isDoorOpen);
                door.GetComponent<SpriteRenderer>().color =
                    _isDoorOpen ? new Color(0, 0, 0, 0) : Color.white;

                npcAtDoor.SetActive(_isDoorOpen);
            }

            Notify(!_isDoorOpen
                ? GameEvents.PlayerApartmentDoorClosed
                : GameEvents.PlayerApartmentDoorOpened);
        }

        private void KnockOnDoor()
        {
            // TODO: Another way then find with string
            var soundEffects = GameObject.Find("📣 Sound Effects Audio Source");

            if (soundEffects)
                soundEffects.GetComponent<SoundEffectsController>().PlaySoundEffect(knockSound);
        }

        private void TriggerKnockOnNpcDoor()
        {
            // this is needed because doors are dynamically instantiated and are not connected to 
            // anything outside floors, like NPCs
            var building = GameObject.Find("🏢 Building controller");
            var buildingController = building.GetComponent<BuildingController>();

            if (building && buildingController)
            {
                print($"Door notifying knock: {doorNumber}");
                buildingController.OnNotify(new GameEventData(GameEvents.KnockOnNpcDoor, doorNumber));
            }
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.OpenNpcDoor)
            {
                _isDoorOpen = true;
                foreach (var door in doors) door.SetActive(true);
            }

            var isDoor = ((string)eventData.Data).ToLower().Contains("inside door") ||
                         ((string)eventData.Data).ToLower().Contains("hallway door");

            if (eventData.Name == GameEvents.ClickOnItem && isDoor)
            {
                print("CLICK ON DOOR");

                if (doorNumber == floorData.playerApartmentNumber)
                {
                    HandlePlayerApartmentDoorClick();
                    return;
                }

                KnockOnDoor();
                TriggerKnockOnNpcDoor();
            }
        }

        public void OpenNpcDoor()
        {
            _isDoorOpen = true;
            foreach (var door in doors) door.SetActive(false);
            // Notify(GameEvents.OpenNpcDoor);
        }

        private void MovePlayerToLinkedDoor()
        {
            Notify(_isPlayerInsideApartment ? GameEvents.ExitApartment : GameEvents.EnterApartment);
            // var x = _isPlayerInsideApartment ? hallwayDoor.transform.position.x : gameObject.transform.position.x;
            // playerTransform.position = new Vector3(x, playerTransform.position.y, playerTransform.position.z);
            _isPlayerInsideApartment = !_isPlayerInsideApartment;
        }

        private void MoveObjectToLinkedDoor(Transform objectTransform)
        {
            var isObjectInsideApartment = Array.Exists(_objectsInsideApartment, obj =>
                obj.transform == objectTransform);
            // var x = isObjectInsideApartment ? hallwayDoor.transform.position.x : gameObject.transform.position.x;
            // objectTransform.position = new Vector3(x, objectTransform.position.y, objectTransform.position.z);

            if (isObjectInsideApartment)
                _objectsInsideApartment = _objectsInsideApartment.Where(obj =>
                    obj.transform != objectTransform).ToArray();
            else
                Array.Resize(ref _objectsInsideApartment, _objectsInsideApartment.Length + 1);

            _objectsInsideApartment[^1] = objectTransform.gameObject;
        }

        public void SetDoorNumber(string newDoorNumber)
        {
            doorNumberTextMeshPro.text = newDoorNumber;
            print($"Setting door number?? {int.Parse(newDoorNumber)}");
            doorNumber = int.Parse(newDoorNumber);
        }
    }
}