using System;
using Dialog.Scripts;
using UnityEngine;

namespace Mini_Games.Phone_Text.scripts
{
    [Serializable]
    public class WifeMessage
    {
        public string text;
    }

    public class PhoneTextMiniGame : MonoBehaviour
    {
        [SerializeField] private GameObject textMessage;
        [SerializeField] private GameObject miniGameContainer;
        [SerializeField] private WifeMessage[] wifeMessages;
        [SerializeField] private PlayerMiniGameChoice[] choices;


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