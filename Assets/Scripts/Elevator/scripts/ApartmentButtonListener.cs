namespace Elevator.scripts
{
    public class ApartmentButtonListener : ObserverSubject
    {
        private void OnMouseDown()
        {
            // TODO: Figure out the optimal way to notify elevator controller
            print("Click on" + gameObject.name);
        }
    }
}