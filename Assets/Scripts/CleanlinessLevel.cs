using System.Collections;
using TMPro;
using UnityEngine;

public class CleanlinessLevel : ObserverSubject
{
    [SerializeField] private TextMeshProUGUI cleanlinessLevelText;
    [SerializeField] private int faucetLevel;
    [SerializeField] private int dirtinessLevel = 100;
    private bool _isInShower;

    private void Start()
    {
        UpdateText(dirtinessLevel);
        StartCoroutine(Cleaning());
    }

    public void OnNotify(GameEventData gameEventData)
    {
        switch (gameEventData.name)
        {
            // case GameEvents.TimerUpdate:
            //     timeToDeath = (int)gameEventData.data;
            //     break;

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

    private IEnumerator Cleaning()
    {
        while (dirtinessLevel > 0)
        {
            if (faucetLevel == 0 || !_isInShower)
                yield return new WaitUntil(() => faucetLevel > 0 && _isInShower);

            yield return new WaitForSeconds((float)1 / faucetLevel);

            dirtinessLevel--;
            UpdateText(dirtinessLevel);
        }

        Notify(GameEvents.IsClean);
    }

    private void UpdateText(int level)
    {
        cleanlinessLevelText.text = $"{level}% DIRTY";
    }
}