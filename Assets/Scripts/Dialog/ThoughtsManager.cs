using System;
using Character_Creator.scripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dialog.Scripts
{
    [Serializable]
    public class PlayerMiniGameChoice
    {
        public string text;
        public int score;
        public NarrationDialogLine actorLine;
    }

    [Serializable]
    public class ThoughtChoice
    {
        public PlayerMiniGameChoice[] choices;
        public bool dropThoughtFromAbove = true;
        public bool randomizeXPosition;
    }

    public class ThoughtsManager : ObserverSubject
    {
        [SerializeField] private GameObject thoughtsContainer;
        [SerializeField] private GameObject thoughtPrefab;
        [SerializeField] private GameObject thoughtsBottom;
        [SerializeField] private GameObject sayingsBottom;
        private string _choice;
        private bool _isSayingsEnabled = true;
        private bool _isThoughtsEnabled = true;
        private PlayerDataEnum _lastPlayerDataType;
        private NarrationDialogLine _nextLine;
        private int _score;

        public void OnNotify(GameEventData gameEventData)
        {
            if (gameEventData.Name is GameEvents.LineNarrationEnd or GameEvents.AddThoughts)
            {
                PlayerChoice[] playerOptionsValue = null;

                // Handle different data structures based on the event type
                if (gameEventData.Name == GameEvents.AddThoughts)
                {
                    // For AddThoughts event, directly access the field from Thought class
                    if (gameEventData.Data is ThoughtChoice thought)
                    {
                        var choices = thought.choices;
                        if (choices == null || choices.Length == 0)
                            return;

                        foreach (var option in choices)
                            CreateThought(
                                option.text,
                                option.actorLine,
                                PlayerDataEnum.None,
                                option.score,
                                thought.dropThoughtFromAbove,
                                thought.randomizeXPosition
                            );
                    }
                }
                else
                {
                    // For LineNarrationEnd event, use reflection to get the property
                    var playerOptions = gameEventData.Data.GetType().GetProperty("playerOptions");
                    if (playerOptions != null)
                        playerOptionsValue = (PlayerChoice[])playerOptions.GetValue(gameEventData.Data);
                }

                if (playerOptionsValue == null || playerOptionsValue.Length == 0)
                    return;

                foreach (var option in playerOptionsValue)
                    CreateThought(option.text, option.next, option.choiceDataType);
            }

            if (gameEventData.Name == GameEvents.ClearThoughts)
            {
                if (_isThoughtsEnabled) DisableThoughts();
                else EnableThoughts();
            }

            if (gameEventData.Name == GameEvents.Speak)
            {
                if (_isSayingsEnabled) Speak();
                else EnableSayings();
            }

            if (gameEventData.Name == GameEvents.KillThoughtsAndSayings)
                KillThoughtsAndSayings();

            if (gameEventData.Name == GameEvents.ResetThoughtsAndSayings)
            {
                KillThoughtsAndSayings();
                EnableSayings();
                EnableThoughts();
            }
        }

        private void CreateThought(
            string text,
            NarrationDialogLine nextLine,
            PlayerDataEnum lastPlayerDataType = PlayerDataEnum.None,
            int score = 0,
            bool dropThoughtFromAbove = true,
            bool randomizeXPosition = false
        )
        {
            var randomHeight = Random.Range(0, 10);
            var xPosition = randomizeXPosition ? Random.Range(-1, 20) : 0;
            var thought = Instantiate(thoughtPrefab, thoughtsContainer.transform);

            if (text.Length <= 1)
            {
                var localScale = thought.transform.localScale;
                localScale.x *= 1f / 3f;
                thought.transform.localScale = localScale;
            }

            if (dropThoughtFromAbove)
                thought.transform.position += new Vector3(xPosition, randomHeight, 0);

            var thoughtComponent = thought.GetComponent<Thought>();
            thoughtComponent.SetThought(text);
            thoughtComponent.SetNextLine(nextLine);
            if (score != 0) thoughtComponent.SetThoughtScore(score);
            if (lastPlayerDataType != PlayerDataEnum.None) thoughtComponent.SetChoicePlayerDataType(lastPlayerDataType);
        }

        private void Speak()
        {
            if (string.IsNullOrEmpty(_choice) || !_nextLine)
            {
                print("No choice given or no next line provided");
                return;
            }

            var choice = new EnrichedPlayerChoice(
                _choice,
                new InteractionData(
                    gameObject.name,
                    InteractableObjectName.Unknown,
                    InteractableObjectType.Unknown,
                    _nextLine
                ),
                _lastPlayerDataType
            );

            Notify(GameEvents.SpeakMind);
            Notify(GameEvents.PlayerClickOnChoice, choice);
            Notify(GameEvents.ThoughtScoreChange, _score);
            DisableSayings();

            _choice = "";
            Invoke(nameof(EnableSayings), 5f);
        }

        private void DisableSayings()
        {
            sayingsBottom.GetComponent<Collider2D>().isTrigger = true;
            sayingsBottom.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            _isSayingsEnabled = false;
        }

        private void EnableSayings()
        {
            sayingsBottom.GetComponent<Collider2D>().isTrigger = false;
            sayingsBottom.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1);
            _isSayingsEnabled = true;
        }

        private void DisableThoughts()
        {
            thoughtsBottom.GetComponent<Collider2D>().isTrigger = true;
            thoughtsBottom.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            _isThoughtsEnabled = false;
        }

        private void EnableThoughts()
        {
            thoughtsBottom.GetComponent<Collider2D>().isTrigger = false;
            thoughtsBottom.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1);
            _isThoughtsEnabled = true;
        }

        public void OnSpeak(string text, NarrationDialogLine nextLine, PlayerDataEnum lastPlayerDataType, int score)
        {
            _choice += text;
            _nextLine = nextLine;
            _lastPlayerDataType = lastPlayerDataType;
            _score = score;

            print($"OnSpeak: {_choice}; Score: {score}");
        }

        public void KillThoughtsAndSayings()
        {
            foreach (Transform child in thoughtsContainer.transform) Destroy(child.gameObject);

            foreach (Transform child in sayingsBottom.transform) Destroy(child.gameObject);
        }
    }
}