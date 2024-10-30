using Character_Creator.scripts;
using UnityEngine;

namespace Dialog.Scripts
{
    public class PlayerChoiceDialogBubbles : ObserverSubject
    {
        [SerializeField] private Transform inputCanvas;
        [SerializeField] private GameObject playerTextInput;
        [SerializeField] private Vector2[] bubblesBasePositions;
        [SerializeField] private GameObject speechBubble;
        private Camera _camera;
        private string _choiceText;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0) || !_camera) return;

            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            var layerMask = LayerMask.GetMask("PlayerChoiceDialogBubbles");

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask))
            {
                var dialogBubbleBehavior = hit.collider.gameObject.GetComponent<DialogBubbleBehavior>();
                var bubbleText = dialogBubbleBehavior.text;
                var bubbleAction = dialogBubbleBehavior.eventAfterChoice;

                foreach (var bubble in GameObject.FindGameObjectsWithTag("PlayerChoiceDialogBubble"))
                    Destroy(bubble);

                foreach (var input in GameObject.FindGameObjectsWithTag("PlayerInput"))
                    Destroy(input);

                var enrichedChoice = new EnrichedPlayerChoice(
                    bubbleText,
                    InteractionStateService.Instance.GetCurrentInteraction()
                );

                Notify(GameEvents.PlayerClickOnChoice, enrichedChoice);
                if (bubbleAction != GameEvents.None) Notify(bubbleAction, enrichedChoice);
            }
        }

        public void OnNotify(GameEventData gameEventData)
        {
            if (gameEventData.name != GameEvents.PlayerChoice)
                return;

            var playerChoices = (PlayerChoice[])gameEventData.data;

            for (var i = 0; i < playerChoices.Length; i++)
            {
                var basePosition = bubblesBasePositions[i];
                var randomX = basePosition.x + Random.Range(-0.1f, 0.1f);
                var randomY = basePosition.y + Random.Range(-0.1f, 0.1f);

                if (playerChoices[i].type == ChoiceType.TextInput)
                {
                    var textInput = Instantiate(playerTextInput, inputCanvas);
                    print($"Setting {playerChoices[i].choiceDataType.ToString()} to {playerChoices[i].text}");
                    textInput.GetComponent<PlayerInfoInput>().type = playerChoices[i].choiceDataType;
                }
                else
                {
                    var bubble = Instantiate(speechBubble, transform);
                    bubble.transform.localPosition = new Vector3(randomX, randomY, transform.position.z);
                    print($"Setting {playerChoices[i].choiceDataType.ToString()} to {playerChoices[i].text}");
                    bubble.GetComponent<PlayerInfoInput>().type = playerChoices[i].choiceDataType;
                    bubble.GetComponent<PlayerInfoInput>().option = playerChoices[i].choiceDataOption;
                    bubble.GetComponent<DialogBubbleBehavior>().text = playerChoices[i].text;
                    bubble.GetComponent<DialogBubbleBehavior>().eventAfterChoice =
                        playerChoices[i].actionAfterPlayerChoice;
                }
            }
        }
    }
}