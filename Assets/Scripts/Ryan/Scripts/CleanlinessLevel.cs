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
    [SerializeField] private TextMeshProUGUI soapinessLevelText;
    [SerializeField] private int faucetLevel;
    [SerializeField] private float dirtinessLevel = 100;
    [SerializeField] private bool isSoapIncluded;
    private float _soapinessLevel;

    private void Start() {
        if (isSoapIncluded) {
            dirtinessLevel = 0;
            faucetLevel = 2;
        }

        UpdateText(dirtinessLevel);
    }

    public void OnNotify(GameEventData gameEventData) {
        switch (gameEventData.name) {
            case GameEvents.IsScrubbing: {
                var playerState = GetPlayerState();
                var throttle = playerState == StateName.Showering ? 1 : 3;

                if (isSoapIncluded) HandleSoapLevel(playerState, throttle);
                else HandleSpongeScrub(playerState, throttle);

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

    private void HandleSoapLevel(StateName playerState, float throttle) {
        if (CursorManager.Instance.IsSoapCursor) {
            if (playerState == StateName.Dressed)
                return;
            _soapinessLevel += 5 * Time.deltaTime;
        }

        if (CursorManager.Instance.IsScrubbingCursor && _soapinessLevel > 0) {
            _soapinessLevel -= 0.1f * faucetLevel / throttle;
            dirtinessLevel += 0.1f * faucetLevel / throttle;
            print("" +
                  $"Soap level: {_soapinessLevel} " +
                  $"Clean level: {dirtinessLevel} " +
                  $"throttle: {throttle} " +
                  $"faucet level: {faucetLevel}");
        }

        soapinessLevelText.text = $"{Mathf.Round(_soapinessLevel)}% Soap'd";

        if (dirtinessLevel >= 100)
            EventManager.Instance.Publish(GameEvents.IsClean);
    }

    private void HandleSpongeScrub(StateName playerState, float throttle) {
        if (
            !CursorManager.Instance.IsScrubbingCursor ||
            !GameState.IsPlayerInShower ||
            faucetLevel <= 0 ||
            playerState == StateName.Dressed
        )
            return;


        dirtinessLevel -= 0.1f * faucetLevel / throttle;

        if (dirtinessLevel <= 0)
            EventManager.Instance.Publish(GameEvents.IsClean);
    }

    private void UpdateText(float level) {
        cleanlinessLevelText.text = isSoapIncluded
            ? $"{Mathf.Round(level)}% Clean"
            : $"{Mathf.Round(level)}% Dirty";
    }

    private StateName GetPlayerState() {
        return PlayerChangeState.Instance.currentState;
    }
}