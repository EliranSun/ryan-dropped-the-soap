public class EventManager : ObserverSubject {
    public static EventManager Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public void Publish(GameEvents eventName) {
        Notify(eventName);
    }

    public void Publish(GameEvents eventName, object data) {
        Notify(eventName, data);
    }
}