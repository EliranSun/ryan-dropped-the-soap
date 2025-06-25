using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    public class PlayerStatesController : ObserverSubject
    {
        [SerializeField] private GameObject playerBox;
        [SerializeField] private GameObject playerPlant;
        [SerializeField] private Camera mainCamera;

        private void Awake()
        {
            // PlayerPrefs.DeleteAll();
            // ChangeScene();
            PositionPlayer();
        }

        private void Start()
        {
            SetPlayerInBox();
            PositionPlant();
        }

        private void PositionPlayer()
        {
            var placePlayerAtElevator = PlayerPrefs.GetInt("PlacePlayerAtElevator");
            var currentSceneName = SceneManager.GetActiveScene().name;

            print("currentSceneName: " + currentSceneName);
            print("placePlayerAtElevator: " + placePlayerAtElevator);

            if (placePlayerAtElevator == 1 && currentSceneName == "hallway scene")
            {
                transform.position = new Vector2(0, transform.position.y);
                mainCamera.GetComponent<Zoom>().startSize = 5;
                mainCamera.GetComponent<Zoom>().endSize = 10;
            }
        }

        private void PositionPlant()
        {
            if (!playerPlant) return;

            var storedPlantPosition = PlayerPrefs.GetString("PlayerPlantPosition");
            var plantPositionParts = storedPlantPosition.Split(',');

            if (
                plantPositionParts.Length == 2 &&
                float.TryParse(plantPositionParts[0], out var x) &&
                float.TryParse(plantPositionParts[1], out var y)
            )
                playerPlant.transform.position = new Vector2(x, y);
        }

        private void SetPlayerInBox()
        {
            if (!playerBox) return;

            var storedIsPlayerOutsideBox = PlayerPrefs.GetInt("PlayerOutsideBox");
            var isPlayerOutsideBox = storedIsPlayerOutsideBox == 1;
            playerBox.SetActive(!isPlayerOutsideBox);

            if (isPlayerOutsideBox) Notify(GameEvents.FreePlayerFromBox);
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
                ChangeScene();
            }

            if (eventData.Name == GameEvents.CharlotteWaitingTheory)
                PlayerPrefs.SetInt("HeardCharlottePlantInstructions", 1);


            if (eventData.Name == GameEvents.PlayerPlacePlant)
            {
                var plantPosition = (Vector3)eventData.Data;
                PlayerPrefs.SetString("PlayerPlantPosition", $"{plantPosition.x},{plantPosition.y}");
            }
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

                case nameof(Location.Hallway):
                {
                    var placePlayerAtElevator = currentScene.name == "inside elevator" ? 1 : 0;
                    print("placePlayerAtElevator: " + placePlayerAtElevator);
                    PlayerPrefs.SetInt("PlacePlayerAtElevator", placePlayerAtElevator);

                    if (currentScene.name != "hallway scene")
                        SceneManager.LoadScene("hallway scene");
                    break;
                }

                case nameof(Location.Elevator):
                    if (currentScene.name != "inside elevator")
                        SceneManager.LoadScene("inside elevator");
                    break;

                case nameof(Location.EmptyApartment):
                    if (currentScene.name != "apartment scene - empty")
                        SceneManager.LoadScene("apartment scene - empty");
                    break;

                case nameof(Location.ZekeApartment):
                    if (currentScene.name != "apartment scene - zeke")
                        SceneManager.LoadScene("apartment scene - zeke");
                    break;

                case nameof(Location.StacyApartment):
                    if (currentScene.name != "apartment scene - stacy")
                        SceneManager.LoadScene("apartment scene - stacy");
                    break;
            }
        }
    }
}