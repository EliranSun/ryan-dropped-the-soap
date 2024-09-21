namespace Character_Creator.scripts
{
    public class OnArmchairClick : ObserverSubject
    {
        private void OnMouseDown()
        {
            Notify(GameEvents.ArmchairClicked, gameObject.name);
        }
    }
}