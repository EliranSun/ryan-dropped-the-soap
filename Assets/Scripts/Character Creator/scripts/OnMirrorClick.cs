namespace Character_Creator.scripts
{
    public class OnMirrorClick : ObserverSubject
    {
        private void OnMouseDown()
        {
            Notify(GameEvents.MirrorClicked, gameObject.name);
        }
    }
}