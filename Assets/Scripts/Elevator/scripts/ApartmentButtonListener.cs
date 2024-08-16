namespace Elevator.scripts
{
    public class ApartmentButtonListener : ObserverSubject
    {
        private void Start()
        {
            // TODO: Sounds like the optimal way here is to add a global event listener
            // instead of attaching a listener to each elevator button, as 
            // there would be a lot of them potentially
            var elevatorController = FindFirstObjectByType<ElevatorController>();
            observers.AddListener(elevatorController.OnNotify);
        }

        private void OnMouseDown()
        {
            print("Click on" + gameObject.name);
            var floorAndApartment = gameObject.name.Split('-')[1];
            var floor = int.Parse(floorAndApartment.Split(':')[0]);

            Notify(GameEvents.ElevatorButtonPress, floor);
        }
    }
}