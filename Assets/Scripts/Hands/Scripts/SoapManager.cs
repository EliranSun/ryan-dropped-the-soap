using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hands.Scripts {
    public class SoapManager : MonoBehaviour {
        private static float _time;
        public static bool IsGrabbingSoap;
        public static bool IsSoapDropped;

        private void Start() {
            // SceneManager.LoadScene("Don't drop the soap", LoadSceneMode.Additive);
        }

        private void Update() {
            if (IsGrabbingSoap) {
                print("GRABBING!");
                _time = Time.time;
            }

            if (IsSoapDropped) {
                print("DROPPED THE SOAP!");
                _time = Time.time;
                IsGrabbingSoap = false;
                IsSoapDropped = false;
                Invoke(nameof(RestartMiniGame), 3);
            }

            if (_time > 3 && IsGrabbingSoap) {
                _time = 0;
                IsGrabbingSoap = false;
                Invoke(nameof(CloseSoapMiniGame), 3);
            }
        }

        private void CloseSoapMiniGame() {
            SceneManager.UnloadSceneAsync("Don't drop the soap");
        }

        private void RestartMiniGame() {
            SceneManager.UnloadSceneAsync("Don't drop the soap");
            SceneManager.LoadScene("Don't drop the soap", LoadSceneMode.Additive);
        }
    }
}