using System;
using UnityEngine;

public enum State
{
    None,
    Dressed,
    Shower,
    Drown,
    Dead
}

[Serializable]
public class GameObjectState
{
    public State state;
    public GameObject gameObject;
    public GameObject position;
}

public class PlayerChangeState : ObserverSubject
{
    [SerializeField] private State state;
    [SerializeField] private GameObjectState[] gameObjectStates;
    private bool _isWaterLevel;

    private void Start()
    {
        ChangePlayerState(state);
        Notify(GameEvents.OutOfShower);
    }

    private void OnMouseDown()
    {
        state = state switch
        {
            State.Dressed => _isWaterLevel ? State.Drown : State.Shower,
            State.Shower => _isWaterLevel ? State.Drown : State.Dressed,
            _ => State.Dressed
        };

        var eventName = state == State.Shower
            ? GameEvents.InShower
            : GameEvents.OutOfShower;

        ChangePlayerState(state);
        Notify(eventName);
    }

    private void ChangePlayerState(State newState)
    {
        foreach (var gameObjectState in gameObjectStates)
            if (gameObjectState.state == newState)
            {
                gameObjectState.gameObject.SetActive(true);
                if (gameObjectState.position)
                    transform.position = gameObjectState.position.transform.position;
            }
            else
            {
                gameObjectState.gameObject.SetActive(false);
            }
    }

    public void OnNotify(GameEventData eventData)
    {
        switch (eventData.name)
        {
            case GameEvents.WaterLevel:
            {
                if (state == State.Dressed)
                {
                    ChangePlayerState(State.Drown);
                    Invoke(nameof(PlayerAvoidableDeath), 5);
                    break;
                }

                PlayerAvoidableDeath();
                break;
            }

            case GameEvents.LevelLost:
                ChangePlayerState(State.Dead);
                break;

            case GameEvents.Drowning:
            {
                if (state == State.Dressed)
                    break;

                ChangePlayerState(State.Drown);
                break;
            }
        }
    }

    private void PlayerAvoidableDeath()
    {
        if (!_isWaterLevel && state == State.Dressed)
            return;

        ChangePlayerState(State.Dead);
    }
}