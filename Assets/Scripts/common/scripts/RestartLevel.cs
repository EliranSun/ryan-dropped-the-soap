using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace common.scripts
{
    public class RestartLevel : MonoBehaviour
    {
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
        }
    }
}