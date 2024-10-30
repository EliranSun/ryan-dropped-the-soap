using Dialog.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace common.scripts
{
    public class RestartLevel : MonoBehaviour
    {
        [SerializeField] private string playerName;
        [SerializeField] private string playerGender;
        [SerializeField] private string playerFeeling;
        [SerializeField] private string playerPartner;

        private void Start()
        {
            playerName = PlayerPrefs.GetString(PlayerDataEnum.Name.ToString());
            playerGender = PlayerPrefs.GetString(PlayerDataEnum.Gender.ToString());
            playerFeeling = PlayerPrefs.GetString(PlayerDataEnum.Feeling.ToString());
            playerPartner = PlayerPrefs.GetString(PlayerDataEnum.Partner.ToString());
        }

        private void Update()
        {
            var currentSelectedInputField =
                EventSystem.current &&
                EventSystem.current.currentSelectedGameObject &&
                EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>();

            if (currentSelectedInputField)
                return;

            if (Input.GetKeyDown(KeyCode.R))
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            if (Input.GetKeyDown(KeyCode.G))
                PlayerPrefs.DeleteKey(PlayerDataEnum.Gender.ToString());

            if (Input.GetKeyDown(KeyCode.N))
                PlayerPrefs.DeleteKey(PlayerDataEnum.Name.ToString());

            if (Input.GetKeyDown(KeyCode.F))
                PlayerPrefs.DeleteKey(PlayerDataEnum.Feeling.ToString());

            if (Input.GetKeyDown(KeyCode.P))
                PlayerPrefs.DeleteKey(PlayerDataEnum.Partner.ToString());
        }
    }
}