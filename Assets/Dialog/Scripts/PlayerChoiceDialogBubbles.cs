using UnityEngine;

namespace Dialog.Scripts
{
    public class PlayerChoiceDialogBubbles : ObserverSubject
    {
        [SerializeField] private Vector2[] bubblesBasePositions;
        [SerializeField] private GameObject speechBubble;
        private string _choiceText;
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
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

                var bubble = Instantiate(speechBubble, transform);

                bubble.transform.localPosition = new Vector3(randomX, randomY, 0f);
                bubble.GetComponent<DialogBubbleBehavior>().text = playerChoices[i].text;
            }
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0)) // Detect left mouse button click
            {
                if (_camera)
                {
                    var ray = _camera.ScreenPointToRay(Input.mousePosition);
                    int layerMask = LayerMask.GetMask("PlayerChoiceDialogBubbles");

                    if (Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask))
                    {
                        var bubbleText = hit.collider.gameObject.GetComponent<DialogBubbleBehavior>().text;
                        foreach (var bubble in GameObject.FindGameObjectsWithTag("PlayerChoiceDialogBubble"))
                            Destroy(bubble);
                        
                        Notify(GameEvents.PlayerClickOnChoice, bubbleText);
                    }
                }
            }
        }
    }
}
