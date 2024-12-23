using UnityEngine;
using UnityEngine.Events;

public class GameEventData
{
    public object data;
    public GameEvents name;

    public GameEventData(GameEvents gameEventName)
    {
        name = gameEventName;
    }

    public GameEventData(GameEvents gameEventName, object data)
    {
        name = gameEventName;
        this.data = data;
    }
}

public class ObserverSubject : MonoBehaviour
{
    public UnityEvent<GameEventData> observers;

    protected void Notify(GameEvents message)
    {
        observers?.Invoke(new GameEventData(message));
    }

    protected void Notify(GameEvents message, object data)
    {
        observers?.Invoke(new GameEventData(message, data));
    }
}