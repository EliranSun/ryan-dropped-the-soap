namespace Character_Creator.scripts
{
    public class OnVaseClick : ObserverSubject
    {
        private void OnMouseDown()
        {
            print("Vase clicked" + gameObject.name);
            Notify(GameEvents.VaseClicked, gameObject.name);
        }
    }
}