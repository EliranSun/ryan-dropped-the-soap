using System.Collections;
using TMPro;
using UnityEngine;

public class CleanlinessLevel : MonoBehaviour {
    [SerializeField] private int timeToDeath;
    [SerializeField] private TextMeshProUGUI cleanlinessLevelText;
    [SerializeField] private int faucetLevel;
    [SerializeField] private int dirtinessLevel = 100;

    private void Start() {
        UpdateText(dirtinessLevel);
        StartCoroutine(Cleaning());
    }

    public void OnNotify(GameEventData gameEventData) {
        if (gameEventData.name == GameEvents.TimerUpdate)
            timeToDeath = (int)gameEventData.data;

        if (gameEventData.name == GameEvents.FaucetOpening) {
            if (faucetLevel >= 5)
                return;

            faucetLevel++;
        }

        if (gameEventData.name == GameEvents.FaucetClosing) {
            if (faucetLevel == 0)
                return;

            faucetLevel--;
        }
    }

    private IEnumerator Cleaning() {
        while (dirtinessLevel > 0) {
            if (faucetLevel == 0)
                yield return new WaitUntil(() => faucetLevel > 0);

            yield return new WaitForSeconds((float)1 / faucetLevel);

            dirtinessLevel--;
            UpdateText(dirtinessLevel);
        }
    }

    private void UpdateText(int level) {
        cleanlinessLevelText.text = level < 50 ? $"{100 - level}% CLEAN" : $"{level}% DIRTY";
    }
}