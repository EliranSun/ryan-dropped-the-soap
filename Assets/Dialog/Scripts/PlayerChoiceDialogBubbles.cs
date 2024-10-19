using UnityEngine;

namespace Dialog.Scripts
{
    public class PlayerChoiceDialogBubbles : MonoBehaviour
    {
        [SerializeField] private Vector2[] bubblesBasePositions;
        [SerializeField] private GameObject speechBubble;

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
    }
}
