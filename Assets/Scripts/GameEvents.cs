﻿public enum GameEvents
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
    MirrorClicked,
    DoorClicked,
    VaseClicked,
    ArmchairClicked,
    EnteredMuseum,
    ExitedMuseum,

    NextScene,
    NextLevel,
    RestartLevel,
    ZoomOut,

    PlayerChoice,
    PlayerClickOnChoice,

    LineNarrationStart,
    LineNarrationEnd,

    ClickOnNpc,
    CountForgettingGuide,

    // museum events
    PaintingChosen,
    MirrorChosen,
    DoorChosen,
    VaseChosen,
    ArmchairChosen,

    CharacterRevealDialogTrigger,
    CharacterRevealTrigger,

    // Apartment intro
    TriggerAlarmClockSound,
    TriggerAlarmClockStop,
    SetClockTime
}