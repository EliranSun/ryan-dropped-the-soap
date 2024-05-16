using System;
using UnityEngine;

public enum State {
    None,
    Dressed,
    Shower,
    Drown,
    Dead
}

[Serializable]
public class GameObjectState {
    public State state;
    public GameObject gameObject;
    public GameObject position;
}

public class PlayerChangeState : ObserverSubject {
    [SerializeField] private State state;
    [SerializeField] private GameObjectState[] gameObjectStates;

    private void Start() {
        ChangeSprite(state);
        Notify(GameEvents.OutOfShower);
    }

    private void OnMouseDown() {
        state = state switch {
            State.Dressed => State.Shower,
            State.Shower => State.Dressed,
            _ => State.Dressed
        };
        var eventName = state == State.Shower
            ? GameEvents.InShower
            : GameEvents.OutOfShower;

        ChangeSprite(state);
        Notify(eventName);
    }

    private void ChangeSprite(State newState) {
        foreach (var gameObjectState in gameObjectStates)
            if (gameObjectState.state == newState) {
                gameObjectState.gameObject.SetActive(true);
                if (gameObjectState.position)
                    transform.position = gameObjectState.position.transform.position;
            }
            else {
                gameObjectState.gameObject.SetActive(false);
            }
    }

    public void OnNotify(GameEventData eventData) {
        if (eventData.name == GameEvents.LevelLost)
            ChangeSprite(State.Dead);
    }
}