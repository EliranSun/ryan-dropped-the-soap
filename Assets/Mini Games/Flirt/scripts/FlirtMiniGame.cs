using System;
using System.Collections;
using System.Linq;
using Dialog;
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
        [SerializeField] private Button[] choiceButtons;
        [SerializeField] private NarrationDialogLine[] initialResponses;
        [SerializeField] private NarrationDialogLine emptyResponse;
        [SerializeField] private PlayerMiniGameChoice[] choices;
        [SerializeField] private SpriteEmotion[] actorSpriteEmotions;
        [SerializeField] private TextMeshProUGUI scoreTextContainer;
        [SerializeField] private Image characterImageContainer;
        [SerializeField] private GameObject[] flirtableNpcs;
        [SerializeField] private Color highlightColor = new(1f, 1f, 0.5f, 1f); // Yellow-ish highlight
        [SerializeField] private float pulseSpeed = 2f; // Speed of the pulsing effect
        [SerializeField] private float pulseScaleMin = 1f; // Minimum scale (normal size)
        [SerializeField] private float pulseScaleMax = 1.15f; // Maximum scale (15% larger)
        private Coroutine[] _highlightCoroutines;
        private int _initResponsesCounter;
        private SpriteRenderer[] _npcSpriteRenderers;
        private Vector3[] _originalScales;

        private void Start()
        {
            // isGameActive = true;
            // StartMiniGame();
        }

        public override void OnNotify(GameEventData eventData)
        {
            base.OnNotify(eventData);

            switch (eventData.Name)
            {
                case GameEvents.MiniGameClosed:
                    isGameActive = false;
                    break;

                case GameEvents.MiniGameIndicationTrigger:
                    var miniGameName = (MiniGameName)eventData.Data;
                    if (miniGameName is MiniGameName.Flirt or MiniGameName.Avoid)
                    {
                        isGameActive = true;
                        StartHighlightingNpcs();
                    }

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

        public void OnButtonChoice(int choiceIndex)
        {
        }

        protected override void StartMiniGame()
        {
            if (!isGameActive)
                return;

            base.StartMiniGame();

            // Randomly select 4 choices from the available choices
            var randomChoices = GetRandomChoices(choices, choiceButtons.Length);

            InitActorResponse();

            // Notify(GameEvents.AddThoughts, new ThoughtChoice
            // {
            //     choices = randomChoices
            // });
            for (var i = 0; i <= choiceButtons.Length - 1; i++)
            {
                var choice = randomChoices[i];
                choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>()
                    .text = choice.text;
            }

            scoreTextContainer.text = score.ToString();
        }

        private void StartHighlightingNpcs()
        {
            if (flirtableNpcs == null || flirtableNpcs.Length == 0)
                return;

            // Initialize arrays
            _highlightCoroutines = new Coroutine[flirtableNpcs.Length];
            _originalScales = new Vector3[flirtableNpcs.Length];
            _npcSpriteRenderers = new SpriteRenderer[flirtableNpcs.Length];

            // Start highlighting coroutine for each NPC
            for (var i = 0; i < flirtableNpcs.Length; i++)
                if (flirtableNpcs[i] != null)
                {
                    _originalScales[i] = flirtableNpcs[i].transform.localScale;
                    _npcSpriteRenderers[i] = flirtableNpcs[i].GetComponent<SpriteRenderer>();
                    _highlightCoroutines[i] = StartCoroutine(HighlightNpcCoroutine(i));
                }
        }

        private void StopHighlightingNpcs()
        {
            if (_highlightCoroutines == null)
                return;

            // Stop all highlighting coroutines
            for (var i = 0; i < _highlightCoroutines.Length; i++)
            {
                if (_highlightCoroutines[i] != null) StopCoroutine(_highlightCoroutines[i]);

                // Reset scale and color
                if (flirtableNpcs != null && i < flirtableNpcs.Length && flirtableNpcs[i] != null)
                {
                    flirtableNpcs[i].transform.localScale = _originalScales[i];
                    if (_npcSpriteRenderers[i] != null) _npcSpriteRenderers[i].color = Color.white;
                }
            }
        }

        private IEnumerator HighlightNpcCoroutine(int npcIndex)
        {
            if (npcIndex >= flirtableNpcs.Length || flirtableNpcs[npcIndex] == null)
                yield break;

            var npc = flirtableNpcs[npcIndex];
            var spriteRenderer = _npcSpriteRenderers[npcIndex];
            var originalScale = _originalScales[npcIndex];
            var originalColor = spriteRenderer != null ? spriteRenderer.color : Color.white;

            var time = 0f;

            while (true)
            {
                time += Time.deltaTime * pulseSpeed;

                // Calculate pulsing scale using sine wave
                var scaleFactor = Mathf.Lerp(pulseScaleMin, pulseScaleMax, (Mathf.Sin(time) + 1f) / 2f);
                npc.transform.localScale = originalScale * scaleFactor;

                // Calculate flashing color using sine wave
                if (spriteRenderer != null)
                {
                    var colorIntensity = (Mathf.Sin(time * 1.5f) + 1f) / 2f; // Slightly faster color pulse
                    spriteRenderer.color = Color.Lerp(originalColor, highlightColor, colorIntensity);
                }

                yield return null;
            }
        }

        private void EndGame()
        {
            print($"Closing the game with score {score}");
            StopHighlightingNpcs();
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