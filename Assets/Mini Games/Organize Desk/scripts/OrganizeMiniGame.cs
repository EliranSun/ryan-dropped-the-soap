using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mini_Games.Organize_Desk.scripts
{
    [Serializable]
    public enum PatternName
    {
        None,
        UShape,
        OShape
    }

    [Serializable]
    public class OrganizationPattern
    {
        public GameObject pattern;
        public PatternName patternName;
    }

    public class OrganizeMiniGame : MonoBehaviour
    {
        [SerializeField] private GameObject[] items;
        [SerializeField] private OrganizationPattern[] patterns;

        private void Start()
        {
            ActivateRandomPattern();
        }

        // Update is called once per frame
        private void Update()
        {
        }

        /// <summary>
        ///     Selects a random pattern from the PatternName enum (excluding None),
        ///     finds the matching pattern in the patterns array, and activates its GameObject.
        /// </summary>
        private void ActivateRandomPattern()
        {
            // Deactivate all patterns first
            foreach (var pattern in patterns)
                if (pattern.pattern != null)
                    pattern.pattern.SetActive(false);

            // Get all pattern names except 'None'
            var patternNames = Enum.GetValues(typeof(PatternName)) as PatternName[];
            if (patternNames == null) return;
            {
                var validPatternNames =
                    patternNames
                        .Where(patternName => patternName != PatternName.None)
                        .ToList();

                // Select a random pattern name
                if (validPatternNames.Count > 0)
                {
                    var randomIndex = Random.Range(0, validPatternNames.Count);
                    var selectedPattern = validPatternNames[randomIndex];

                    Debug.Log($"Selected random pattern: {selectedPattern}");

                    // Find and activate the matching pattern
                    foreach (var pattern in patterns)
                        if (pattern.patternName == selectedPattern && pattern.pattern != null)
                        {
                            pattern.pattern.SetActive(true);
                            Debug.Log($"Activated pattern: {selectedPattern}");
                            break;
                        }
                }
                else
                {
                    Debug.LogWarning("No valid patterns found to activate.");
                }
            }
        }
    }
}