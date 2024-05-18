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

    private void ChangePlayerState(StateName newState) {
        foreach (var state in states)
            state.spriteObject.gameObject.SetActive(state.name == newState);
    }

    public void OnNotify(GameEventData eventData) {
        switch (eventData.name) {
            case GameEvents.FaucetClosed:
                ChangePlayerState(StateName.Default);
                break;

            case GameEvents.FaucetOpening:
                ChangePlayerState(StateName.Showering);
                break;

            case GameEvents.WaterEverywhere: {
                _isRoomWaterFilled = true;

                ChangePlayerState(StateName.Drowning);
                Invoke(nameof(PlayerAvoidableDeath), 5);
                break;
            }

            case GameEvents.Drowning: {
                _isShowerWaterFilled = true;

                if (currentState != StateName.Drowning) {
                    ChangePlayerState(StateName.Drowning);
                    Invoke(nameof(PlayerAvoidableDeath), 5);
                }

                break;
            }

            case GameEvents.LevelLost:
                HandleDeath();
                break;
        }
    }

    private void PlayerAvoidableDeath() {
        // if (!_isRoomWaterFilled && currentState == StateName.OutOfShower)
        //     return;

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