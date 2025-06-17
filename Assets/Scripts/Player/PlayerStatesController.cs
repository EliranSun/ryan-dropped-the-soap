using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    public class PlayerStatesController : MonoBehaviour
    {
        [SerializeField] private GameObject playerBox;

        private void Awake()
        {
            ChangeScene();
        }

        private void Start()
        {
            SetPlayerInBox();
            SetPosition();
        }

        private void SetPlayerInBox()
        {
            var storedIsPlayerInBox = PlayerPrefs.GetInt("IsPlayerInBox");
            playerBox.SetActive(storedIsPlayerInBox == 1);
        }

        private void SetPosition()
        {
            var storedPosition = PlayerPrefs.GetString("PlayerPosition");
            var positionParts = storedPosition.Split(',');

            if (
                positionParts.Length == 2 &&
                float.TryParse(positionParts[0], out var x) &&
                float.TryParse(positionParts[1], out var y)
            )
                transform.position = new Vector2(x, y);
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.ChangePlayerLocation)
            {
                var newLocation = (Location)eventData.Data;
                PlayerPrefs.SetString("PlayerLocation", newLocation.ToString());
                playerBox.SetActive(newLocation == Location.PlayerApartment);
                ChangeScene();
            }

            if (eventData.Name == GameEvents.CharlotteWaitingTheory)
                PlayerPrefs.SetInt("HeardCharlottePlantInstructions", 1);
        }

        private void ChangeScene()
        {
            var currentScene = SceneManager.GetActiveScene();
            switch (PlayerPrefs.GetString("PlayerLocation"))
            {
                case nameof(Location.BuildingFrontView):
                    if (currentScene.name != "3a. building front scene")
                        SceneManager.LoadScene("3a. building front scene");
                    break;

                case nameof(Location.PlayerApartment):
                    if (currentScene.name != "3b. inside apartment")
                        SceneManager.LoadScene("3b. inside apartment");
                    break;
            }
        }
    }
}