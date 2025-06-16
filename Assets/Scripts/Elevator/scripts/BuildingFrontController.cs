using Player;

namespace Elevator.scripts
{
    public class BuildingFrontController : ObserverSubject
    {
        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.ClickOnItem)
                // SceneManager.LoadScene("3b. inside apartment");
                Notify(GameEvents.ChangePlayerLocation, Location.PlayerApartment);
        }
    }
}