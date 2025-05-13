using System;
using System.Linq;
using Common;
using TMPro;
using UnityEngine;

namespace common.scripts
{
    [RequireComponent(typeof(Collider2D))]
    public class DoorController : ObserverSubject
    {
        [SerializeField] private TextMeshPro doorNumberTextMeshPro;
        [SerializeField] private TransitionController transitionImage;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private GameObject hallwayDoor;
        [SerializeField] private FloorData floorData;
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
            print("CLICK ON DOOR:" + _doorNumber);
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

        private void MovePlayerToLinkedDoor()
        {
            Notify(_isPlayerInsideApartment ? GameEvents.ExitApartment : GameEvents.EnterApartment);
            var x = _isPlayerInsideApartment ? hallwayDoor.transform.position.x : gameObject.transform.position.x;
            playerTransform.position = new Vector3(x, playerTransform.position.y, playerTransform.position.z);
            _isPlayerInsideApartment = !_isPlayerInsideApartment;
        }

        private void MoveObjectToLinkedDoor(Transform objectTransform)
        {
            var isObjectInsideApartment = Array.Exists(_objectsInsideApartment, obj =>
                obj.transform == objectTransform);
            var x = isObjectInsideApartment ? hallwayDoor.transform.position.x : gameObject.transform.position.x;
            objectTransform.position = new Vector3(x, objectTransform.position.y, objectTransform.position.z);

            if (isObjectInsideApartment)
                _objectsInsideApartment = _objectsInsideApartment.Where(obj =>
                    obj.transform != objectTransform).ToArray();
            else
                Array.Resize(ref _objectsInsideApartment, _objectsInsideApartment.Length + 1);
            _objectsInsideApartment[^1] = objectTransform.gameObject;
        }

        public void OnNotify(GameEventData gameEventData)
        {
            if (gameEventData.Name != GameEvents.ClickOnItem) return;

            var itemName = (string)gameEventData.Data;
            if (!itemName.ToLower().Contains("door")) return;


            _isDoorOpen = !_isDoorOpen;
            hallwayDoor.SetActive(!_isDoorOpen);
            Notify(!_isDoorOpen
                ? GameEvents.PlayerApartmentDoorClosed
                : GameEvents.PlayerApartmentDoorOpened);
        }

        public void SetDoorNumber(string doorNumber)
        {
            doorNumberTextMeshPro.text = doorNumber;
            _doorNumber = int.Parse(doorNumber);
        }
    }
}