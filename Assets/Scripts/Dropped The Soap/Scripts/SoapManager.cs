using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dropped_The_Soap.Scripts {
    public class SoapManager : MonoBehaviour {
        private static float _time;
        public static bool IsGrabbingSoap;
        public static bool IsSoapDropped;
        [SerializeField] private TextMeshProUGUI textIndication;

        private void Update() {
            if (IsGrabbingSoap && _time == 0) _time = Time.time;

            if (IsSoapDropped) {
                if (textIndication) textIndication.text = "YOU DROPPED IT!";
                _time = Time.time;
                IsGrabbingSoap = false;
                IsSoapDropped = false;
                Invoke(nameof(RestartMiniGame), 3);
            }

            if (_time > 0 && Time.time - _time > 3 && IsGrabbingSoap) {
                _time = 0;
                IsGrabbingSoap = false;
                if (textIndication) textIndication.text = "GRABBED IT!";
                Invoke(nameof(CloseSoapMiniGame), 3);
            }
        }

        private void CloseSoapMiniGame() {
            EventManager.Instance.Publish(GameEvents.SoapMiniGameWon);
            SceneManager.UnloadSceneAsync("Don't drop the soap");
        }

        private void RestartMiniGame() {
            try {
                EventManager.Instance.Publish(GameEvents.SoapMiniGameLost);
                SceneManager.UnloadSceneAsync("Don't drop the soap");
            }
            catch (ArgumentException error) {
                print("Scene already unloaded");
            }

            // SceneManager.LoadScene("Don't drop the soap", LoadSceneMode.Additive);
        }
    }
}