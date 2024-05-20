using System.Collections;
using TMPro;
using UnityEngine;

public class CleanlinessLevel : ObserverSubject {
    [SerializeField] private TextMeshProUGUI cleanlinessLevelText;
    [SerializeField] private int faucetLevel;
    [SerializeField] private float dirtinessLevel = 100;
    private bool _isInShower;
    private int _maxDirtiness;

    private void Start() {
        _maxDirtiness = (int)dirtinessLevel;
        UpdateText(dirtinessLevel);
        // StartCoroutine(Cleaning());
    }

    public void OnNotify(GameEventData gameEventData) {
        switch (gameEventData.name) {
            // case GameEvents.TimerUpdate:
            //     timeToDeath = (int)gameEventData.data;
            //     break;

            case GameEvents.IsScrubbing:
                print(
                    $"GameEvents.IsScrubbing _isInShower {_isInShower}, faucetLevel {faucetLevel}, dirtinessLevel {dirtinessLevel}");
                if (_isInShower && faucetLevel > 0) {
                    if (dirtinessLevel <= 0) {
                        Notify(GameEvents.IsClean);
                        break;
                    }

                    dirtinessLevel -= 0.1f * faucetLevel;
                    UpdateText(dirtinessLevel);
                }

                break;

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

            case GameEvents.OutOfShower:
                _isInShower = false;
                break;

            case GameEvents.InShower:
                _isInShower = true;
                break;
        }
    }

    private IEnumerator Cleaning() {
        while (dirtinessLevel > 0) {
            if (faucetLevel == 0 || !_isInShower)
                yield return new WaitUntil(() => faucetLevel > 0 && _isInShower);

            yield return new WaitForSeconds((float)1 / faucetLevel);

            dirtinessLevel--;
            UpdateText(dirtinessLevel);
        }

        Notify(GameEvents.IsClean);
    }

    private void UpdateText(float level) {
        cleanlinessLevelText.text = $"{Mathf.Round(level / _maxDirtiness * 100)}% DIRTY";
    }
}