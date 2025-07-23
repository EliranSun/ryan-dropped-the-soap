using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Player
{
    public class PlayerStatesController : ObserverSubject
    {
        [SerializeField] private GameObject playerBox;
        [SerializeField] private GameObject playerPlant;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private GameObject gun;
        [SerializeField] private GameObject[] paintings;
        [SerializeField] private bool resetPlayerPrefs;
        private bool _allowGun;

        private InputAction _attackAction;

        private void Awake()
        {
            if (resetPlayerPrefs)
                PlayerPrefs.DeleteAll();
            // ChangeScene();
            PositionPlayer();
        }

        private void Start()
        {
            _attackAction = InputSystem.actions.FindAction("Attack");
            if (gun) gun.SetActive(false);

            SetPlayerBoxState();
            SetPlayerHoldingPainting();
        }

        private void Update()
        {
            if (gun && _attackAction.IsPressed() && _allowGun && !gun.activeSelf)
            {
                gun.SetActive(true);
                Notify(GameEvents.GunIsOut);
            }
        }

        private void SetPlayerHoldingPainting()
        {
            var storedPainting = PlayerPrefs.GetString("PlayerHoldingItem", "");
            if (paintings is { Length: > 0 } && storedPainting != "")
                paintings.First(p => p.name == storedPainting).SetActive(true);
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

        private void SetPlayerBoxState()
        {
            if (!playerBox)
                return;

            var storedIsPlayerOutsideBox = PlayerPrefs.GetInt("PlayerOutsideBox");
            var isPlayerOutsideBox = storedIsPlayerOutsideBox == 1;
            playerBox.SetActive(!isPlayerOutsideBox);
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
            if (eventData.Name == GameEvents.FreePlayerFromBox)
            {
                print("Freeing player from box");
                PlayerPrefs.SetInt("PlayerOutsideBox", 1);
                SetPlayerBoxState();
            }

            if (eventData.Name == GameEvents.ChangePlayerLocation)
            {
                var newLocation = (Location)eventData.Data;
                PlayerPrefs.SetString("PlayerLocation", newLocation.ToString());
                ChangeScene();
            }

            if (eventData.Name == GameEvents.CharlotteWaitingTheory)
                PlayerPrefs.SetInt("HeardCharlottePlantInstructions", 1);

            if (eventData.Name == GameEvents.AllowGun)
                _allowGun = true;

            switch (eventData.Name)
            {
                case GameEvents.PlayerPlacePlant:
                case GameEvents.PlayerPlaceMirror:
                case GameEvents.PlayerPlacePainting:
                {
                    var itemPosition = (Vector3)eventData.Data;
                    PlayerPrefs.SetString($"{eventData.Name}Position", $"{itemPosition.x},{itemPosition.y}");

                    if (eventData.Name == GameEvents.PlayerPlacePainting)
                        PlayerPrefs.DeleteKey("PlayerHoldingItem");
                    break;
                }
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
                    if (currentScene.name != "apartment scene - player")
                        SceneManager.LoadScene("apartment scene - player");
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