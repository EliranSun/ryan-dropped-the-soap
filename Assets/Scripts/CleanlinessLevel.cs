using System.Collections;
using TMPro;
using UnityEngine;

public class CleanlinessLevel : ObserverSubject {
    [SerializeField] private int timeToDeath;
    [SerializeField] private TextMeshProUGUI cleanlinessLevelText;
    [SerializeField] private int faucetLevel;
    [SerializeField] private int dirtinessLevel = 100;
    private bool _isOutOfShower = true;

    private void Start() {
        UpdateText(dirtinessLevel);
        StartCoroutine(Cleaning());
    }

    public void OnNotify(GameEventData gameEventData) {
        switch (gameEventData.name) {
            case GameEvents.TimerUpdate:
                timeToDeath = (int)gameEventData.data;
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
                _isOutOfShower = true;
                break;

            case GameEvents.InShower:
                _isOutOfShower = false;
                break;

            case GameEvents.TimeIsUp:
                Invoke(nameof(HandleTimeUp), 3);
                break;
        }
    }

    private void HandleTimeUp() {
        Notify(dirtinessLevel == 0 ? GameEvents.LevelWon : GameEvents.LevelLost);
    }

    private IEnumerator Cleaning() {
        while (dirtinessLevel > 0) {
            if (faucetLevel == 0 || _isOutOfShower)
                yield return new WaitUntil(() => faucetLevel > 0 && !_isOutOfShower);

            yield return new WaitForSeconds((float)1 / faucetLevel);

            dirtinessLevel--;
            UpdateText(dirtinessLevel);
        }
    }

    private void UpdateText(int level) {
        cleanlinessLevelText.text = $"{level}% DIRTY";
    }
}