using UnityEngine;
using UnityEngine.Events;

public enum GameEvents
{
    // Dropped the soap scenes events
    None,
    FaucetOpening,
    FaucetClosing,
    TriggerNonStick,
    TriggerStick,
    DownwardsControllerMotion,
    UpwardsControllerMotion,
    StrongPull,
    Pumping,
    TimerUpdate,
    InShower,
    OutOfShower,
    TimeIsUp,
    LevelWon,
    LevelLost,
    IsClean,
    FaucetClosed,
    WaterFilledShower,
    WaterEverywhere,
    IsScrubbing,
    Dead,
    HitByConcrete,
    SoapMiniGameWon,
    SoapMiniGameLost,
    SoapDropped,
    PickedItem,
    DroppedItem,

    // elevator/floor events
    ElevatorButtonPress,
    ElevatorReachedFloor,
    ElevatorMoving,
    FloorChange,
    FloorsUpdate,
    ExitElevatorToFloors,
    ExitElevatorToLobby,
    PlayerInsideElevator,

    // museum events
    PaintingClicked,
    MirrorClicked
}

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