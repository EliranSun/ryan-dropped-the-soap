using UnityEngine;
using UnityEngine.Events;

namespace Observer {
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
}