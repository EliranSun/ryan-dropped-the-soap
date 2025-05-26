using System;
using System.Linq;
using common.scripts;
using Common.scripts;
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
        private int _doorNumber;
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
            {
                transitionImage.FadeInOut();
                // Invoke(nameof(MoveObjectToLinkedDoor), 0.3f);
                MovePlayerToLinkedDoor();
            }
        }

        private void OnMouseDown()
        {
            if (_doorNumber == floorData.playerApartmentNumber)
            {
                // player apartment
                print("Changing player door state");
                _isDoorOpen = !_isDoorOpen;
                foreach (var door in doors) door.SetActive(!_isDoorOpen);
                Notify(!_isDoorOpen
                    ? GameEvents.PlayerApartmentDoorClosed
                    : GameEvents.PlayerApartmentDoorOpened);
                return;
            }

            // npc apartment
            KnockOnDoor();
            TriggerKnockOnNpcDoor();
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
                // if (_isDoorOpen)
                //     // Notify(GameEvents.EnterHallway);
                //     _isPlayerInsideApartment = !_isPlayerInsideApartment;
                _isPlayerOnDoor = false;
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
            var eventController = GameObject.Find("🏢 Building controller");
            if (eventController && eventController.GetComponent<BuildingController>())
                eventController
                    .GetComponent<BuildingController>()
                    .OnNotify(new GameEventData(GameEvents.KnockOnNpcDoor, _doorNumber));
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.OpenNpcDoor)
            {
                _isDoorOpen = true;
                foreach (var door in doors) door.SetActive(true);
            }
        }

        public void OpenNpcDoor()
        {
            _isDoorOpen = true;
            foreach (var door in doors) door.SetActive(true);
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

        public void SetDoorNumber(string doorNumber)
        {
            doorNumberTextMeshPro.text = doorNumber;
            _doorNumber = int.Parse(doorNumber);
        }
    }
}