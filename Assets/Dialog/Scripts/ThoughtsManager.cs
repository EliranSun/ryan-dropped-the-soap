using Character_Creator.scripts;
using UnityEngine;

namespace Dialog.Scripts
{
    public class ThoughtsManager : ObserverSubject
    {
        [SerializeField] private GameObject thoughtsContainer;
        [SerializeField] private GameObject thoughtPrefab;
        [SerializeField] private Collider2D thoughtsBottomCollider;
        [SerializeField] private Collider2D sayingsBottomCollider;

        public void OnNotify(GameEventData gameEventData)
        {
            if (gameEventData.name == GameEvents.LineNarrationEnd)
            {
                var playerOptions = gameEventData.data.GetType().GetProperty("playerOptions");
                if (playerOptions == null)
                    return;

                var playerOptionsValue = (PlayerChoice[])playerOptions.GetValue(gameEventData.data);
                if (playerOptionsValue == null)
                    return;

                foreach (var option in playerOptionsValue)
                {
                    var randomHeight = Random.Range(0, 5);
                    var thought = Instantiate(thoughtPrefab, thoughtsContainer.transform);
                    thought.transform.position += new Vector3(0, randomHeight, 0);
                    thought.GetComponent<Thought>().SetThought(option.text);
                    thought.GetComponent<Thought>().SetNextLine(option.next);
                }

                Invoke(nameof(RemoveThoughts), 10f);
            }
        }

        public void RemoveThoughts()
        {
            thoughtsBottomCollider.enabled = false;
            Invoke(nameof(RemoveSayings), 4f);
        }

        public void RemoveSayings()
        {
            sayingsBottomCollider.enabled = false;
        }

        public void OnSpeak(string text, NarrationDialogLine nextLine)
        {
            var choice = new EnrichedPlayerChoice(
                text,
                new InteractionData(
                    gameObject.name,
                    InteractableObjectName.Unknown,
                    InteractableObjectType.Unknown,
                    nextLine
                ));

            Notify(GameEvents.PlayerClickOnChoice, choice);
        }
    }
}