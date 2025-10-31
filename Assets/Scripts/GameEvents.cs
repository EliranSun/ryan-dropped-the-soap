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
    SetClockTime,

    ObjectClicked,

    ExitApartment,
    EnterApartment,
    EnterHallway,

    CharacterCreatorNextSetOfObjects,
    CharacterCreatorHideObjects,

    PlayerEnrichedChoice,

    ShapeClicked,
    DependentClicked,

    KillDependents,

    CharlotteBeachDialogInit,
    CharlotteBeachDialogStart,
    CharlotteBeachDialogEnd,
    ClearDialogImageOverlay,

    ClearThoughts,
    Speak,

    BoatStart,

    AddThoughts,

    ActorReaction,
    ThoughtScoreChange,
    KillThoughtsAndSayings,
    KillDialog,
    FlirtGameStart,
    MiniGameIndicationTrigger,
    TextMessageGameStart,
    MiniGameClosed,
    UIItemClicked,
    UIItemPlacementClicked,
    CollisionEnter,
    CollisionExit,
    MiniGameWon,
    MiniGameLost,
    MiniGameStart,
    TriggerSpecificDialogLine,
    DeskItemsChanged,
    ResetThoughtsAndSayings,
    StartMiniGames,
    SlowDownMusic,
    SpeedUpMusic,
    StopMusic,
    StartMusic,
    BossOfficeDialogStart,
    TriggerZekeShout,
    TriggerZekeBossStub,
    SpeakMind,
    EndZekeShouts,
    TriggerZekeSuicide,
    FirstTimePlayerMove,
    ZekeSuicideEnd,
    ClickOnItem,
    RopeNeededDialog,
    HeadBackDialog,
    DimLight,
    BrightenLight,
    ZoomChangeEnd,
    ZoomStart,
    CrawlTrigger,
    IdleTrigger,

    FlashlightClicked,
    FlashlightPicked,
    FlashlightDropped,
    SandwichFed,
    SandwichConsumed,

    AutoMovementTrigger,
    NpcAtBridge,

    RevealKnife,
    NpcDeath,
    EnableRope,
    RopeAttached,
    RopeDetached,
    RopeInHand,
    RopeInBag,

    TreasureFound,

    GlobalLightToggle,

    PlayerApartmentDoorClosed,
    PlayerApartmentDoorOpened,

    KnockOnPlayerApartment,
    CeaseKnocking,
    FreePlayerFromBox,
    PlayerHoldPlant,
    CharlotteGavePlayerPlant,
    CharlotteWaitingTheory,
    PlayerPlacePlant,
    PlayerGrowth,
    PlayerGrew,

    FollowPlayer,

    TriggerSoundEffect,

    OpenNpcDoor,
    KnockOnNpcDoor,

    PlayerOutsideTrigger,
    ChangePlayerLocation,

    HideSelf,
    ShowSelf,

    StartElevatorFinalSequence,
    StopElevator,
    ResumeElevator,

    PlayerPlacePainting,
    PlayerPlaceMirror,

    AllowGun,
    GunIsOut,
    MurderedNpc,

    NpcAtPlayerDoor,
    AddPointOfInterest,
    NpcGoTo,
    ExitPlayerApartment,

    PaintingChosenKiss,
    PaintingChosenComposition,
    PaintingChosenWanderer,
    PaintingChosenNighthawks,
    PaintingChosenStarryNight,
    PaintingChosenAngelus,
    PaintingChosenImpression,

    MirrorChosenSamaritan,
    MirrorChosenEgyptian,
    MirrorChosenRoman,
    MirrorChosenGreek,

    PlayerReleaseItem,

    PlayerInteraction,
    EnterTheBuilding,
    ExitTheBuilding,

    CallElevator,
    EnterElevator,

    CameraTransitionEnded,
    DisablePlayerMovement,
    EnablePlayerMovement,
    ChangeActiveLayer,

    TriggerLockPickMiniGame,

    RyanPullGun,
    StacyPullGun,

    ThoughtDrag,
    ThoughtDrop
}