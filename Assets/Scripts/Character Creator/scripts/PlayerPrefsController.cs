using System;
using Dialog.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Character_Creator.scripts
{
    [Serializable]
    public class PlayerInformation
    {
        [SerializeField] public string playerName;
        [SerializeField] public string playerGender;
        [SerializeField] public string playerFeeling;
        [SerializeField] public string playerPartner;
        [SerializeField] public string door;
        [SerializeField] public string painting;
        [SerializeField] public string armchair;
        [SerializeField] public string shape;
        [SerializeField] public string mirror;
        [SerializeField] public string vase;
        [SerializeField] public string dependent;
    }

    public class PlayerPrefsController : MonoBehaviour
    {
        [SerializeField] private PlayerInformation playerInformation;
        [SerializeField] private RotateObjectsAroundAxis characterCreatorObjectsController;

        private void Awake()
        {
            PopulatePlayerInformation();
            SetCharacterCreatorObjectsIndex();
        }

        private void Update()
        {
            var currentSelectedInputField =
                EventSystem.current &&
                EventSystem.current.currentSelectedGameObject &&
                EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>();

            if (currentSelectedInputField)
                return;

            if (Input.GetKeyDown(KeyCode.G))
                PlayerPrefs.DeleteKey(PlayerDataEnum.Gender.ToString());

            if (Input.GetKeyDown(KeyCode.N))
                PlayerPrefs.DeleteKey(PlayerDataEnum.Name.ToString());

            if (Input.GetKeyDown(KeyCode.F))
                PlayerPrefs.DeleteKey(PlayerDataEnum.Feeling.ToString());

            if (Input.GetKeyDown(KeyCode.P))
                PlayerPrefs.DeleteKey(PlayerDataEnum.Partner.ToString());
        }

        private void PopulatePlayerInformation()
        {
            playerInformation.playerName = PlayerPrefs.GetString(PlayerDataEnum.Name.ToString());
            playerInformation.playerGender = PlayerPrefs.GetString(PlayerDataEnum.Gender.ToString());
            playerInformation.playerFeeling = PlayerPrefs.GetString(PlayerDataEnum.Feeling.ToString());
            playerInformation.playerPartner = PlayerPrefs.GetString(PlayerDataEnum.Partner.ToString());
            playerInformation.door = PlayerPrefs.GetString(PlayerDataEnum.Door.ToString());
            playerInformation.painting = PlayerPrefs.GetString(PlayerDataEnum.Painting.ToString());
            playerInformation.armchair = PlayerPrefs.GetString(PlayerDataEnum.Armchair.ToString());
            playerInformation.shape = PlayerPrefs.GetString(PlayerDataEnum.Shape.ToString());
            playerInformation.mirror = PlayerPrefs.GetString(PlayerDataEnum.Mirror.ToString());
            playerInformation.vase = PlayerPrefs.GetString(PlayerDataEnum.Vase.ToString());
            playerInformation.dependent = PlayerPrefs.GetString(PlayerDataEnum.Dependent.ToString());
        }

        public void OnNotify(GameEventData gameEventData)
        {
            if (gameEventData.name != GameEvents.PlayerEnrichedChoice)
                return;

            var playerData = (InteractionData)gameEventData.data;
            PlayerPrefs.SetString(playerData.InteractableObjectType.ToString(),
                playerData.InteractableObjectName.ToString());
        }

        private void SetCharacterCreatorObjectsIndex()
        {
            var objectIndexPairs = new[]
            {
                (value: playerInformation.door, index: 0),
                (value: playerInformation.painting, index: 1),
                (value: playerInformation.armchair, index: 2),
                (value: playerInformation.shape, index: 3), // partner
                (value: playerInformation.vase, index: 4),
                (value: playerInformation.dependent, index: 5),
                (value: playerInformation.mirror, index: 6),
            };

            foreach (var (value, index) in objectIndexPairs)
            {
                if (value != string.Empty) continue;
                if (characterCreatorObjectsController != null)
                {
                    characterCreatorObjectsController.activePrefabsIndex = index;
                }
                return;
            }
        }
    }
}