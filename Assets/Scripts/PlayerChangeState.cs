using System;
using UnityEngine;

public enum StateName {
    Default,
    Showering,
    Drowning,
    Dead
}

[Serializable]
public class States {
    public StateName name;
    public GameObject spriteObject;
}

public class PlayerChangeState : ObserverSubject {
    [SerializeField] private GameObject player;
    [SerializeField] private StateName currentState;
    [SerializeField] private States[] states;

    private bool _isRoomWaterFilled;
    private bool _isShowerWaterFilled;

    private void Start() {
        ChangePlayerState(currentState);
    }

    private void Update() {
        switch (GameState.WaterFilledShower) {
            case true when !_isShowerWaterFilled: {
                _isShowerWaterFilled = true;
                if (GameState.IsPlayerInShower && currentState != StateName.Drowning) {
                    ChangePlayerState(StateName.Drowning);
                    Invoke(nameof(PlayerAvoidableDeath), 5);
                }

                break;
            }
            case false when _isShowerWaterFilled: {
                _isShowerWaterFilled = false;
                if (GameState.IsPlayerInShower && currentState == StateName.Drowning)
                    ChangePlayerState(StateName.Showering);
                break;
            }
        }

        switch (GameState.WaterFilledRoom) {
            case true when !_isRoomWaterFilled: {
                _isRoomWaterFilled = true;
                if (currentState != StateName.Drowning) {
                    ChangePlayerState(StateName.Drowning);
                    Invoke(nameof(PlayerAvoidableDeath), 5);
                }

                break;
            }
            case false when _isRoomWaterFilled: {
                _isRoomWaterFilled = false;
                if (currentState == StateName.Drowning)
                    ChangePlayerState(GameState.IsPlayerInShower ? StateName.Showering : StateName.Default);
                break;
            }
        }
    }

    private void ChangePlayerState(StateName newState) {
        currentState = newState;
        foreach (var state in states)
            state.spriteObject.gameObject.SetActive(state.name == newState);
    }

    public void OnNotify(GameEventData eventData) {
        switch (eventData.name) {
            case GameEvents.FaucetClosed:
                if (_isRoomWaterFilled)
                    break;

                if (_isShowerWaterFilled && GameState.IsPlayerInShower)
                    break;

                ChangePlayerState(StateName.Default);
                break;

            case GameEvents.FaucetOpening:
                if (_isRoomWaterFilled)
                    break;

                if (_isShowerWaterFilled && GameState.IsPlayerInShower)
                    break;

                ChangePlayerState(StateName.Showering);
                break;

            case GameEvents.InShower: {
                if (currentState == StateName.Default && _isShowerWaterFilled)
                    ChangePlayerState(StateName.Drowning);

                break;
            }

            case GameEvents.OutOfShower: {
                if (currentState == StateName.Drowning && !_isRoomWaterFilled)
                    ChangePlayerState(StateName.Default);

                break;
            }

            case GameEvents.LevelLost:
                HandleDeath();
                break;
        }
    }

    private void PlayerAvoidableDeath() {
        if ((!_isRoomWaterFilled && !GameState.IsPlayerInShower) ||
            (!_isShowerWaterFilled && GameState.IsPlayerInShower))
            return;

        HandleDeath();
    }

    private void HandleDeath() {
        ChangePlayerState(StateName.Dead);
        Invoke(nameof(NotifyDeath), 5);
    }

    private void NotifyDeath() {
        Notify(GameEvents.Dead);
    }
}