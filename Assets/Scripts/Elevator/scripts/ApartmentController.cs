using Player;
using UnityEngine;

namespace Elevator.scripts
{
    public class ApartmentController : ObserverSubject
    {
        [SerializeField] private DoorController door;
        private int _apartmentNumber;
        private int _floorNumber;

        public void SetData(int floorNum, int apartmentNum)
        {
            _floorNumber = floorNum;
            _apartmentNumber = apartmentNum;

            door.SetDoorNumber($"{floorNum}{apartmentNum}");

            // TODO: Refactor to match new building
            // if (tenant.floorNumber == _floorNumber && tenant.apartmentNumber == _apartmentNumber)
            // {
            //     
            // }
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.ClickOnItem)
                Notify(GameEvents.ChangePlayerLocation, Location.BuildingFrontView);
        }
    }
}