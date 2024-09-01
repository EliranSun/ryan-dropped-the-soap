namespace Character_Creator.scripts
{
    public class OnPaintingClick : ObserverSubject
    {
        private void OnMouseDown()
        {
            Notify(GameEvents.PaintingClicked, gameObject.name);
        }
    }
}