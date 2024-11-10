using Dialog.Scripts;
using UnityEngine;

namespace common.scripts
{
    [RequireComponent(typeof(Collider2D))]
    public class DoorInteraction : ObserverSubject
    {
        private bool _isDoorOpen;
        private bool _isPlayerInsideApartment = true;
        private bool _isPlayerNear;

        // Update is called once per frame
        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.X))
                return;

            if (_isDoorOpen && _isPlayerNear)
                Notify(_isPlayerInsideApartment ? GameEvents.ExitApartment : GameEvents.EnterApartment);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                _isPlayerNear = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (_isDoorOpen)
                    // Notify(GameEvents.EnterHallway);
                    _isPlayerInsideApartment = !_isPlayerInsideApartment;

                _isPlayerNear = false;
            }
        }

        public void OnNotify(GameEventData gameEventData)
        {
            if (gameEventData.name != GameEvents.ObjectClicked)
                return;

            if ((InteractableObjectType)gameEventData.data == InteractableObjectType.ApartmentDoorKnob)
                _isDoorOpen = !_isDoorOpen;
        }
    }
}