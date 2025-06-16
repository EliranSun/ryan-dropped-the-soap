using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    public class PlayerStatesController : MonoBehaviour
    {
        [SerializeField] private PlayerScriptableObject playerData;
        [SerializeField] private GameObject playerBox;

        private void Awake()
        {
            ChangeScene();
        }

        private void Start()
        {
            playerBox.SetActive(playerData.isPlayerInBox);
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.ChangePlayerLocation)
            {
                var newLocation = (Location)eventData.Data;
                playerData.location = newLocation;
                playerBox.SetActive(playerData.isPlayerInBox);
                ChangeScene();
            }
        }

        private void ChangeScene()
        {
            var currentScene = SceneManager.GetActiveScene();
            switch (playerData.location)
            {
                case Location.BuildingFrontView:
                    if (currentScene.name != "3a. building front scene")
                        SceneManager.LoadScene("3a. building front scene");
                    break;

                case Location.PlayerApartment:
                    if (currentScene.name != "3b. inside apartment")
                        SceneManager.LoadScene("3b. inside apartment");
                    break;
            }
        }
    }
}