using UnityEngine;

namespace Mini_Games.Phone_Text.scripts
{
    public class PhoneTextMiniGame : MonoBehaviour
    {
        [SerializeField] private GameObject textMessage;
        [SerializeField] private GameObject miniGameContainer;


        private void Start()
        {
            miniGameContainer.SetActive(false);
        }

        // Update is called once per frame
        private void Update()
        {
        }
    }
}