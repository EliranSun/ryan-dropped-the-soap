using Player;

namespace Elevator.scripts
{
    public class BuildingFrontController : ObserverSubject
    {
        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.ClickOnItem)
                Notify(GameEvents.ChangePlayerLocation, Location.PlayerApartment);
        }
    }
}