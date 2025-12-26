using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Dialog;
using Expressions;
using interactions;
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

    [Serializable]
    public class LineResponse
    {
        public NarrationDialogLine line;
        public PlayerMiniGameChoice[] choices;
    }

    [Serializable]
    public class ActorLines
    {
        public ActorName actorName;
        public Image characterImageContainer;
        public NarrationDialogLine line;

        public PlayerMiniGameChoice[] choices; // public LineResponse defaultLine;
        // public LineResponse[] lines;
    }

    public class EngageMiniGame : MiniGame
    {
        [Header("Actors")] [SerializeField] private GameObject[] flirtableNpcs;

        [SerializeField] private SpriteEmotion[] actorSpriteEmotions;
        [SerializeField] private TextMeshProUGUI actorScoreTextMesh;

        [Header("Dialog")] [SerializeField] private Button[] choiceButtons;

        [SerializeField] private List<ActorLines> actorsLines = new();
        // [SerializeField] private PlayerMiniGameChoice[] choices;

        [Header("UI")] [SerializeField] private Color highlightColor = new(1f, 1f, 0.5f, 1f); // Yellow-ish highlight

        [SerializeField] private float pulseSpeed = 2f; // Speed of the pulsing effect
        [SerializeField] private float pulseScaleMin = 1f; // Minimum scale (normal size)
        [SerializeField] private float pulseScaleMax = 1.15f; // Maximum scale (15% larger)

        // should be a different domain
        private readonly Dictionary<ActorName, float> _actorScores = new() { { ActorName.Morgan, 0 } };
        private ActorName _currentActor;

        private PlayerMiniGameChoice[] _currentChoices;
        private Coroutine[] _highlightCoroutines;
        private int _initResponsesCounter;
        private LineResponse _npcCurrentLine;
        private SpriteRenderer[] _npcSpriteRenderers;
        private Vector3[] _originalScales;

        private void OnEnable()
        {
            StartMiniGame();
        }

        private void OnDisable()
        {
            EndGame();
        }

        public override void OnNotify(GameEventData eventData)
        {
            base.OnNotify(eventData);

            switch (eventData.Name)
            {
                case GameEvents.PlayerInteractionRequest:
                    var interaction = (ActorInteraction)eventData.Data;
                    if (interaction.type == ObjectInteractionType.Flirt)
                        StartMiniGame(interaction.actorName);
                    break;

                case GameEvents.MiniGameStart:
                    PopulateChoiceButtons();
                    ToggleChoiceButtons(true);
                    break;

                case GameEvents.MiniGameIndicationTrigger:
                    var miniGameNameTrigger = (MiniGameName)eventData.Data;
                    if (miniGameNameTrigger is MiniGameName.Flirt or MiniGameName.Avoid)
                        StartHighlightingNpcs();
                    break;

                case GameEvents.FlirtGameStart:
                    EndGame();
                    StartMiniGame();
                    break;

                case GameEvents.ActorReaction:
                    var dialogLine = eventData.Data as NarrationDialogLine;

                    if (!dialogLine) return;
                    if (dialogLine.actorName != _currentActor)
                        return;

                    var reaction = actorSpriteEmotions.First(x =>
                        x.reaction == dialogLine.actorReaction);

                    // characterImageContainer.sprite = reaction.sprite;
                    break;
            }
        }

        public void OnButtonChoice(int choiceIndex)
        {
            var choice = _currentChoices[choiceIndex];
            UpdateActorScore(choice.score);
            Notify(GameEvents.TriggerSpecificDialogLine, choice.actorLine);
            Invoke(nameof(EndGame), 3);
        }

        private void UpdateActorScore(float score)
        {
            if (actorScoreTextMesh)
            {
                _actorScores[_currentActor] += score;
                actorScoreTextMesh.SetText(_actorScores[_currentActor].ToString(CultureInfo.InvariantCulture));
            }
        }

        private void StartMiniGame(ActorName actorName)
        {
            base.StartMiniGame();
            _currentActor = actorName;
            InitActorResponse();
            ToggleChoiceButtons(false);
            // PopulateChoiceButtons();
        }

        private void ToggleChoiceButtons(bool enable)
        {
            for (var i = 0; i <= choiceButtons.Length - 1; i++)
            {
                var button = choiceButtons[i].gameObject;
                button.SetActive(enable);
            }
        }

        private void PopulateChoiceButtons()
        {
            // Randomly select 4 choices from the available choices
            // _currentChoices = GetRandomChoices(_npcCurrentLine.choices, choiceButtons.Length);
            // for (var i = 0; i <= choiceButtons.Length - 1; i++)
            // {
            //     var choice = _currentChoices[i];
            //     choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>()
            //         .text = choice.text;
            // }
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
            StopHighlightingNpcs();
            CloseMiniGame(true);
        }

        private void InitActorResponse()
        {
            var actorLines = actorsLines.Find(item => item.actorName == _currentActor);
            // _npcCurrentLine = actorLines.defaultLine;
            // if (_initResponsesCounter <= actorLines.lines.Length - 1)
            //     _npcCurrentLine = actorLines.lines[_initResponsesCounter];

            Notify(GameEvents.TriggerSpecificDialogLine, _npcCurrentLine.line);
            _initResponsesCounter++;
        }

        // Helper method to randomly select n choices from the available choices
        private PlayerMiniGameChoice[] GetRandomChoices(PlayerMiniGameChoice[] availableChoices, int count)
        {
            // Make sure we don't try to select more choices than available
            count = Mathf.Min(count, availableChoices.Length);

            // Create a copy of the available choices to avoid modifying the original array
            var choicesCopy = availableChoices.ToArray();

            // Shuffle the array using Fisher-Yates algorithm:O
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