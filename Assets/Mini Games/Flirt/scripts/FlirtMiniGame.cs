using System;
using System.Linq;
using Dialog;
using Dialog.Scripts;
using Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Mini_Games.Flirt.scripts
{
    [Serializable]
    internal class SpriteEmotion
    {
        public Sprite sprite;
        public Expression reaction;
    }


    public class FlirtMiniGame : MiniGame
    {
        [SerializeField] private ActorName actorName = ActorName.Morgan;
        [SerializeField] private NarrationDialogLine[] initialResponses;
        [SerializeField] private NarrationDialogLine emptyResponse;
        [SerializeField] private PlayerMiniGameChoice[] choices;
        [SerializeField] private SpriteEmotion[] actorSpriteEmotions;
        [SerializeField] private TextMeshProUGUI scoreTextContainer;
        [SerializeField] private Image characterImageContainer;
        private int _initResponsesCounter;

        public override void OnNotify(GameEventData eventData)
        {
            base.OnNotify(eventData);

            switch (eventData.Name)
            {
                case GameEvents.MiniGameClosed:
                    isGameActive = false;
                    break;

                case GameEvents.MiniGameIndicationTrigger when
                    (MiniGameName)eventData.Data == MiniGameName.Flirt:
                    isGameActive = true;
                    break;
            }

            if (!isGameActive)
                return;

            switch (eventData.Name)
            {
                case GameEvents.FlirtGameStart:
                    EndGame();
                    StartMiniGame();
                    break;

                case GameEvents.ActorReaction:
                    var dialogLine = eventData.Data as NarrationDialogLine;

                    if (!dialogLine) return;
                    if (dialogLine.actorName != actorName) return;

                    var reaction = actorSpriteEmotions.First(x =>
                        x.reaction == dialogLine.actorReaction);

                    characterImageContainer.sprite = reaction.sprite;
                    break;

                case GameEvents.ThoughtScoreChange:
                    var newScore = (int)eventData.Data;
                    if (newScore != 0)
                    {
                        score = newScore;
                        print($"Flirt mini game new score: {newScore}");
                        scoreTextContainer.text = score.ToString();

                        if (score is <= 0 or >= 100)
                            Invoke(nameof(EndGame), 2);
                    }

                    break;
            }
        }

        protected override void StartMiniGame()
        {
            if (!isGameActive)
                return;

            base.StartMiniGame();

            // Randomly select 4 choices from the available choices
            var randomChoices = GetRandomChoices(choices, 4);

            InitActorResponse();

            Notify(GameEvents.AddThoughts, new ThoughtChoice
            {
                choices = randomChoices
            });
            scoreTextContainer.text = score.ToString();
        }

        private void EndGame()
        {
            print($"Closing the game with score {score}");
            CloseMiniGame(score > 0);
        }

        private void InitActorResponse()
        {
            var response = emptyResponse;
            if (_initResponsesCounter <= initialResponses.Length - 1)
                response = initialResponses[_initResponsesCounter];

            Notify(GameEvents.TriggerSpecificDialogLine, response);
            _initResponsesCounter++;
        }

        // Helper method to randomly select n choices from the available choices
        private PlayerMiniGameChoice[] GetRandomChoices(PlayerMiniGameChoice[] availableChoices, int count)
        {
            // Make sure we don't try to select more choices than available
            count = Mathf.Min(count, availableChoices.Length);

            // Create a copy of the available choices to avoid modifying the original array
            var choicesCopy = availableChoices.ToArray();

            // Shuffle the array using Fisher-Yates algorithm
            for (var i = choicesCopy.Length - 1; i > 0; i--)
            {
                var randomIndex = Random.Range(0, i + 1);
                (choicesCopy[i], choicesCopy[randomIndex]) = (choicesCopy[randomIndex], choicesCopy[i]);
            }

            // Return the first 'count' elements from the shuffled array
            return choicesCopy.Take(count).ToArray();
        }
    }
}