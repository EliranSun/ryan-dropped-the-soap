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
        private string _choice;
        private NarrationDialogLine _nextLine;

        public void OnNotify(GameEventData gameEventData)
        {
            if (gameEventData.name == GameEvents.LineNarrationEnd)
            {
                var playerOptions = gameEventData.data.GetType().GetProperty("playerOptions");
                if (playerOptions == null)
                    return;

                var playerOptionsValue = (PlayerChoice[])playerOptions.GetValue(gameEventData.data);
                if (playerOptionsValue == null || playerOptionsValue.Length == 0)
                    return;

                foreach (var option in playerOptionsValue)
                {
                    var randomHeight = Random.Range(0, 5);
                    var thought = Instantiate(thoughtPrefab, thoughtsContainer.transform);
                    thought.transform.position += new Vector3(0, randomHeight, 0);
                    thought.GetComponent<Thought>().SetThought(option.text);
                    thought.GetComponent<Thought>().SetNextLine(option.next);
                }
            }

            if (gameEventData.name == GameEvents.ClearThoughts) RemoveThoughts();
            if (gameEventData.name == GameEvents.Speak) Speak();
        }

        private void RemoveThoughts()
        {
            thoughtsBottomCollider.enabled = false;
            Invoke(nameof(Speak), 4f);
            Invoke(nameof(RestartThoughtsBottomCollider), 1f);
        }

        public void Speak()
        {
            var choice = new EnrichedPlayerChoice(
                _choice,
                new InteractionData(
                    gameObject.name,
                    InteractableObjectName.Unknown,
                    InteractableObjectType.Unknown,
                    _nextLine
                ));

            Notify(GameEvents.PlayerClickOnChoice, choice);
            sayingsBottomCollider.enabled = false;
            Invoke(nameof(RestartSayingBottomCollider), 1f);
        }

        private void RestartSayingBottomCollider()
        {
            sayingsBottomCollider.enabled = true;
        }

        private void RestartThoughtsBottomCollider()
        {
            thoughtsBottomCollider.enabled = true;
        }

        public void OnSpeak(string text, NarrationDialogLine nextLine)
        {
            _choice += text;
            _nextLine = nextLine;
        }
    }
}