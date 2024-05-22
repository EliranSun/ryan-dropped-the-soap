using System;
using System.Linq;
using UnityEngine;

public enum StateName {
    Dressed,
    Naked,
    Showering,
    Drowning,
    Dead
}

[Serializable]
public class States {
    public StateName name;
    public GameObject spriteObject;
    public GameObject[] additionalObjects;
    public bool isControlledByPlayer;
}

public class PlayerChangeState : MonoBehaviour {
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject clothing;
    [SerializeField] public StateName currentState;
    [SerializeField] private States[] states;
    [SerializeField] private States[] controlledByPlayerStates;
    private int _activeStateIndex;
    private bool _isRoomWaterFilled;
    private bool _isShowerWaterFilled;

    public static PlayerChangeState Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start() {
        controlledByPlayerStates = states.Where(state => state.isControlledByPlayer).ToArray();
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
                    ChangePlayerState(GameState.IsPlayerInShower ? StateName.Showering : StateName.Dressed);
                break;
            }
        }
    }

    private void OnMouseDown() {
        if (CursorManager.Instance.IsActionCursor)
            return;

        _activeStateIndex = _activeStateIndex + 1 > controlledByPlayerStates.Length - 1
            ? _activeStateIndex = 0
            : _activeStateIndex + 1;


        var nextState = controlledByPlayerStates[_activeStateIndex];
        ChangePlayerState(nextState.name);
    }

    private void ChangePlayerState(StateName newState) {
        if (newState == StateName.Naked)
            clothing.gameObject.SetActive(true);

        if (newState == StateName.Dressed)
            clothing.gameObject.SetActive(false);

        foreach (var state in states)
            state.spriteObject.gameObject.SetActive(state.name == newState);

        currentState = newState;
    }

    public void OnNotify(GameEventData eventData) {
        switch (eventData.name) {
            case GameEvents.InShower: {
                if (currentState == StateName.Dressed && _isShowerWaterFilled)
                    ChangePlayerState(StateName.Drowning);
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
        EventManager.Instance.Publish(GameEvents.Dead);
    }
}