using System;
using System.Collections;
using System.Linq;
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

public class CleanlinessLevel : ObserverSubject {
    [SerializeField] private TextMeshProUGUI cleanlinessLevelText;
    [SerializeField] private int faucetLevel;
    [SerializeField] private float dirtinessLevel = 100;
    [SerializeField] private Position[] positions;
    private int _maxDirtiness;

    private void Start() {
        _maxDirtiness = (int)dirtinessLevel;
        UpdateText(dirtinessLevel);
    }

    public void OnNotify(GameEventData gameEventData) {
        switch (gameEventData.name) {
            case GameEvents.IsScrubbing: {
                var isInShower = IsInShower();
                var playerState = GetPlayerState();
                var throttle = playerState == StateName.Showering ? 1 : 3;

                if (!isInShower || faucetLevel <= 0 || playerState == StateName.Dressed)
                    break;

                if (dirtinessLevel <= 0) {
                    Notify(GameEvents.IsClean);
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

    private IEnumerator Cleaning() {
        while (dirtinessLevel > 0) {
            var isInShower = IsInShower();
            if (faucetLevel == 0 || !isInShower)
                yield return new WaitUntil(() => faucetLevel > 0 && isInShower);

            yield return new WaitForSeconds((float)1 / faucetLevel);

            dirtinessLevel--;
            UpdateText(dirtinessLevel);
        }

        Notify(GameEvents.IsClean);
    }

    private void UpdateText(float level) {
        cleanlinessLevelText.text = $"{Mathf.Round(level / _maxDirtiness * 100)}% DIRTY";
    }

    private bool IsInShower() {
        var playerTransform = transform;
        var inShowerTransform = positions.First(position => position.name == PositionName.InShower).transform;

        return Vector2.Distance(playerTransform.position, inShowerTransform.position) < 1;
    }

    private StateName GetPlayerState() {
        return PlayerChangeState.Instance.currentState;
    }
}