using UnityEngine;
using UnityEngine.Events;

public class GameEventData
{
    public readonly object Data;
    public GameEvents Name;

    public GameEventData(GameEvents gameEventName)
    {
        Name = gameEventName;
    }

    public GameEventData(GameEvents gameEventName, object data)
    {
        Name = gameEventName;
        Data = data;
    }
}

public class ObserverSubject : MonoBehaviour
{
    [SerializeField] private GameEvents eventName;
    [Header("Observers")] public UnityEvent<GameEventData> observers;

    protected void Notify(GameEvents message)
    {
        observers?.Invoke(new GameEventData(message));
    }

    protected void Notify(GameEvents message, object data)
    {
        observers?.Invoke(new GameEventData(message, data));
    }
}