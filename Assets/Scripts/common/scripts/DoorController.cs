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
        private int _doorNumber;
        private bool _isDoorOpen;
        private bool _isPlayerInsideApartment = true;
        private bool _isPlayerOnDoor;

        // Update is called once per frame
        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.X))
                return;

            if (_isPlayerOnDoor && _isDoorOpen)
            {
                transitionImage.FadeInOut();
                Invoke(nameof(MovePlayerToLinkedDoor), 0.3f);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                _isPlayerOnDoor = true;
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

        public void OnNotify(GameEventData gameEventData)
        {
            if (gameEventData.Name == GameEvents.ClickOnItem)
            {
                var itemName = (string)gameEventData.Data;
                if (itemName.ToLower().Contains("door"))
                {
                    _isDoorOpen = !_isDoorOpen;
                    hallwayDoor.SetActive(!_isDoorOpen);
                    Notify(!_isDoorOpen
                        ? GameEvents.PlayerApartmentDoorClosed
                        : GameEvents.PlayerApartmentDoorOpened);
                }
            }
        }

        public void SetDoorNumber(string doorNumber)
        {
            doorNumberTextMeshPro.text = doorNumber;
        }
    }
}