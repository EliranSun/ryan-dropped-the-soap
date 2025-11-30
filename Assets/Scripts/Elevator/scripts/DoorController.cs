using System;
using System.Linq;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Elevator.scripts
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(AudioSource))]
    public class DoorController : ObserverSubject
    {
        [SerializeField] private TextMeshPro doorNumberTextMeshPro;
        [SerializeField] private TextMeshPro instructionsTextMeshPro;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private FloorData floorData;
        [SerializeField] public int doorNumber;
        [SerializeField] private bool isPlayerApartment;
        [SerializeField] private GameObject door;
        [SerializeField] private GameObject entrance;
        [SerializeField] private AudioClip[] knockSounds;

        private AudioSource _audioSource;
        private bool _isDoorOpen;
        private bool _isPlayerInsideApartment = true;
        private bool _isPlayerOnDoor;
        private GameObject[] _objectsInsideApartment = { };

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();

            _isPlayerInsideApartment =
                SceneManager.GetActiveScene().name.ToLower().Contains("apartment");

            var playerStatesController = FindFirstObjectByType<PlayerStatesController>();
            if (playerStatesController != null) observers.AddListener(playerStatesController.OnNotify);

            if (instructionsTextMeshPro) instructionsTextMeshPro.text = "";
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X) && _isPlayerOnDoor)
            {
                print($"Is door open? {_isDoorOpen}; Is player apartment? {isPlayerApartment}");
                // if (isDoorOpen) EnterApartment();
                if (isPlayerApartment) OpenDoor();
                else KnockOnDoor();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerOnDoor = true;
                if (instructionsTextMeshPro)
                    instructionsTextMeshPro.text = isPlayerApartment
                        ? _isDoorOpen ? "CLOSE" : "OPEN"
                        : "KNOCK";
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerOnDoor = false;
                if (instructionsTextMeshPro) instructionsTextMeshPro.text = "";
            }
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.OpenNpcDoor)
            {
                var openDoorNumber = (int)eventData.Data;
                if (openDoorNumber == doorNumber) OpenDoor();
            }
        }

        private void EnterApartment()
        {
            // var location = doorNumber switch
            // {
            //     var n when n == floorData.PlayerApartmentNumber => Location.PlayerApartment,
            //     var n when n == floorData.ZekeApartmentNumber => Location.ZekeApartment,
            //     var n when n == floorData.StacyApartmentNumber => Location.StacyApartment,
            //     _ => Location.EmptyApartment
            // };
            //
            // print($"Enter/Exit apartment {doorNumber}, {location}");
            //
            // PlayerPrefs.SetInt("ExitFromApartment", doorNumber);
            //
            // Notify(GameEvents.ChangePlayerLocation,
            //     _isPlayerInsideApartment ? Location.Hallway : location);
        }

        private void KnockOnDoor()
        {
            _audioSource.PlayOneShot(knockSounds[Random.Range(0, knockSounds.Length)]);
            TriggerKnockOnNpcDoor();
        }

        // private void HandlePlayerApartmentDoorClick()
        // {
        //     _isDoorOpen = !_isDoorOpen;
        //
        //     ToggleDoorState();
        //
        //     Notify(_isDoorOpen
        //         ? GameEvents.PlayerApartmentDoorOpened
        //         : GameEvents.PlayerApartmentDoorClosed);
        // }


        private void TriggerKnockOnNpcDoor()
        {
            // this is needed because doors are dynamically instantiated and are not connected to 
            // anything outside floors, like NPCs
            // var building = GameObject.Find("🏢 Building controller");
            // if (!building)
            //     return;

            // var buildingController = building.GetComponent<BuildingController>();
            // if (!buildingController)
            //     return;

            // buildingController.OnNotify(new GameEventData(GameEvents.KnockOnNpcDoor, doorNumber));
            Notify(GameEvents.KnockOnNpcDoor, doorNumber);
        }

        // public void OnNotify(GameEventData eventData)
        // {
        //     if (eventData.Name == GameEvents.OpenNpcDoor)
        //     {
        //         isDoorOpen = true;
        //         ToggleDoorState();
        //     }
        //
        //     if (eventData.Name == GameEvents.ClickOnItem)
        //     {
        //         var isDoor =
        //             ((string)eventData.Data).ToLower().Contains("inside door") ||
        //             ((string)eventData.Data).ToLower().Contains("hallway door");
        //
        //         if (!isDoor)
        //             return;
        //
        //         if (doorNumber == floorData.PlayerApartmentNumber)
        //             HandlePlayerApartmentDoorClick();
        //     }
        // }

        public void OpenDoor()
        {
            if (_isDoorOpen) return;
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
            doorNumber = int.Parse(newDoorNumber);
        }

        private void ToggleDoorState()
        {
            _isDoorOpen = !_isDoorOpen;
            // TODO: flip door from each side
            var spriteRenderer = door.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                spriteRenderer.flipX = _isDoorOpen;
            transform.Translate(new Vector2(_isDoorOpen ? 4 : -4, 0));
            var nonTriggerCollider = door.GetComponents<Collider2D>().FirstOrDefault(c => !c.isTrigger);
            if (nonTriggerCollider != null)
                nonTriggerCollider.enabled = !_isDoorOpen;

            // door.GetComponent<SpriteRenderer>().enabled = !_isDoorOpen;
            // entrance.GetComponent<Collider2D>().enabled = _isDoorOpen;

            // if (doorNumberTextMeshPro)
            //     doorNumberTextMeshPro.enabled = !_isDoorOpen;
        }
    }
}