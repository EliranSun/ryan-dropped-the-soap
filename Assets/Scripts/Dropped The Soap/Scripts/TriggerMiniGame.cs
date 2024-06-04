using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Dropped_The_Soap.Scripts {
    public class TriggerMiniGame : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI textIndication;

        private void OnMouseDown() {
            var shouldTriggerMiniGame = Random.Range(0, 100) > 33;
            if (shouldTriggerMiniGame) {
                if (textIndication) textIndication.text = "SLIPPED! CATCH IT!";
                SceneManager.LoadScene("Don't drop the soap", LoadSceneMode.Additive);
            }
            else {
                if (textIndication) textIndication.text = "GRABBED!";
                EventManager.Instance.Publish(GameEvents.SoapMiniGameWon);
            }

            Invoke(nameof(ClearText), 2);
        }

        private void ClearText() {
            if (textIndication) textIndication.text = "";
        }
    }
}