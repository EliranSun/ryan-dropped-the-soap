using UnityEngine;
using UnityEngine.Events;

public enum GameEvents {
    None,
    FaucetOpening,
    FaucetClosing
}

public class ObserverSubject : MonoBehaviour {
    public UnityEvent<GameEvents> observers;

    protected void Notify(GameEvents message) {
        observers?.Invoke(message);
    }
}