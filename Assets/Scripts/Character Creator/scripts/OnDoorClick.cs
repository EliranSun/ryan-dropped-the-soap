namespace Character_Creator.scripts
{
    public class OnDoorClick : ObserverSubject
    {
        private void OnMouseDown()
        {
            Notify(GameEvents.DoorClicked, gameObject.name);
        }
    }
}