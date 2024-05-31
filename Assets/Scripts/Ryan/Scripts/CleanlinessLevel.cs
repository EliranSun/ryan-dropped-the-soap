using System;
using Ryan.Scripts;
using TMPro;
using UnityEngine;

[Serializable]
public enum PositionName {
    None,
    InShower,
    OutOfShower
}

[Serializable]
public class Position {
    public PositionName name;
    public Transform transform;
}

public class CleanlinessLevel : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI cleanlinessLevelText;
    [SerializeField] private int faucetLevel;
    [SerializeField] private float dirtinessLevel = 100;
    private int _maxDirtiness;

    private void Start() {
        _maxDirtiness = (int)dirtinessLevel;
        UpdateText(dirtinessLevel);
    }

    public void OnNotify(GameEventData gameEventData) {
        switch (gameEventData.name) {
            case GameEvents.IsScrubbing: {
                var playerState = GetPlayerState();
                var throttle = playerState == StateName.Showering ? 1 : 3;

                if (!GameState.IsPlayerInShower || faucetLevel <= 0 || playerState == StateName.Dressed)
                    break;

                if (dirtinessLevel <= 0) {
                    EventManager.Instance.Publish(GameEvents.IsClean);
                    break;
                }

                dirtinessLevel -= 0.1f * faucetLevel / throttle;
                UpdateText(dirtinessLevel);
                break;
            }

            case GameEvents.FaucetOpening when faucetLevel >= 5:
                return;

            case GameEvents.FaucetOpening:
                faucetLevel++;
                break;

            case GameEvents.FaucetClosing when faucetLevel == 0:
                return;

            case GameEvents.FaucetClosing:
                faucetLevel--;
                break;
        }
    }

    private void UpdateText(float level) {
        cleanlinessLevelText.text = $"{Mathf.Round(level / _maxDirtiness * 100)}% DIRTY";
    }

    private StateName GetPlayerState() {
        return PlayerChangeState.Instance.currentState;
    }
}