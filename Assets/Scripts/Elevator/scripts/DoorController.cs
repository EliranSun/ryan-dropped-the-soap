using System;
using System.Linq;
using common.scripts;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        [SerializeField] public int doorNumber;
        [SerializeField] private GameObject npcAtDoor;
        [SerializeField] private bool isDoorOpen;
        private AudioSource _audioSource;
        private bool _isPlayerInsideApartment = true;
        private bool _isPlayerOnDoor;
        private GameObject[] _objectsInsideApartment = { };

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            ToggleDoorState();

            _isPlayerInsideApartment =
                SceneManager.GetActiveScene().name.ToLower().Contains("apartment");

            var playerStatesController = FindFirstObjectByType<PlayerStatesController>();
            if (playerStatesController != null) observers.AddListener(playerStatesController.OnNotify);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                print($"Is player on door? {_isPlayerOnDoor}; is door open? {isDoorOpen}");

                if (!_isPlayerOnDoor || !isDoorOpen)
                    return;

                var location = doorNumber switch
                {
                    var n when n == floorData.PlayerApartmentNumber => Location.PlayerApartment,
                    var n when n == floorData.ZekeApartmentNumber => Location.ZekeApartment,
                    var n when n == floorData.StacyApartmentNumber => Location.StacyApartment,
                    _ => Location.EmptyApartment
                };

                print(location);

                Notify(GameEvents.ChangePlayerLocation,
                    _isPlayerInsideApartment ? Location.Hallway : location);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) _isPlayerOnDoor = true;

            if (other.CompareTag("NPC"))
                MoveObjectToLinkedDoor(other.transform);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player")) _isPlayerOnDoor = false;
        }

        private void HandlePlayerApartmentDoorClick()
        {
            isDoorOpen = !isDoorOpen;

            print($"Door is {isDoorOpen}");

            ToggleDoorState();

            Notify(isDoorOpen
                ? GameEvents.PlayerApartmentDoorOpened
                : GameEvents.PlayerApartmentDoorClosed);
        }


        private void TriggerKnockOnNpcDoor()
        {
            // this is needed because doors are dynamically instantiated and are not connected to 
            // anything outside floors, like NPCs
            var building = GameObject.Find("🏢 Building controller");
            if (!building)
                return;

            var buildingController = building.GetComponent<BuildingController>();
            if (!buildingController)
                return;

            buildingController.OnNotify(new GameEventData(GameEvents.KnockOnNpcDoor, doorNumber));
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.OpenNpcDoor)
            {
                isDoorOpen = true;
                ToggleDoorState();
            }


            if (eventData.Name == GameEvents.ClickOnItem)
            {
                var isDoor = ((string)eventData.Data).ToLower().Contains("inside door") ||
                             ((string)eventData.Data).ToLower().Contains("hallway door");

                if (!isDoor)
                    return;

                if (doorNumber == floorData.PlayerApartmentNumber)
                {
                    HandlePlayerApartmentDoorClick();
                    return;
                }

                if (isDoorOpen)
                    return;

                _audioSource.PlayOneShot(knockSound);
                TriggerKnockOnNpcDoor();
            }
        }

        public void OpenNpcDoor()
        {
            isDoorOpen = true;
            ToggleDoorState();
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

        private void ToggleDoorState()
        {
            if (npcAtDoor)
                npcAtDoor.SetActive(isDoorOpen);

            foreach (var door in doors)
                door.GetComponent<SpriteRenderer>().enabled = !isDoorOpen;
        }
    }
}