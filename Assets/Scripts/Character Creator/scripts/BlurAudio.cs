using Dialog;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character_Creator.scripts
{
    [RequireComponent(typeof(AudioSource))]
    public class BlurAudio : MonoBehaviour
    {
        [FormerlySerializedAs("_defaultCutoffFrequency")] [SerializeField]
        private float defaultCutoffFrequency = 22000f; // Default cutoff (no filter)

        [FormerlySerializedAs("_minCutoffFrequency")] [SerializeField]
        private float minCutoffFrequency = 400f; // Heavy muffling

        private AudioSource _audioSource;
        private AudioLowPassFilter _lowPassFilter;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();

            // Add low pass filter if it doesn't exist
            _lowPassFilter = GetComponent<AudioLowPassFilter>();
            if (_lowPassFilter == null)
            {
                _lowPassFilter = gameObject.AddComponent<AudioLowPassFilter>();
                _lowPassFilter.cutoffFrequency = defaultCutoffFrequency;
            }
        }

        /// <summary>
        ///     Applies a muffled effect to the audio based on the blur level
        /// </summary>
        /// <param name="blurLevel">0 = no blur, 1 = maximum blur</param>
        private void ApplyBlurEffect(float blurLevel)
        {
            // Clamp blur level between 0 and 1
            blurLevel = Mathf.Clamp01(blurLevel);

            if (blurLevel <= 0)
            {
                // No blur - set to default (high) cutoff frequency
                _lowPassFilter.cutoffFrequency = defaultCutoffFrequency;
            }
            else
            {
                // Calculate cutoff frequency based on blur level
                // As blur increases, cutoff frequency decreases (more muffled)
                var cutoffFrequency = Mathf.Lerp(defaultCutoffFrequency, minCutoffFrequency, blurLevel);
                _lowPassFilter.cutoffFrequency = cutoffFrequency;
            }
        }

        public void OnNotify(GameEventData gameEventData)
        {
            if (gameEventData.Name == GameEvents.LineNarrationStart)
            {
                var narrationDialogLine = gameEventData.Data as NarrationDialogLine;
                if (narrationDialogLine == null) return;

                if (narrationDialogLine.blurLevel > 0)
                    ApplyBlurEffect(narrationDialogLine.blurLevel);

                print($"Blur audio for {narrationDialogLine.name}? {narrationDialogLine.blurAudio}");
                if (narrationDialogLine.blurAudio)
                    ApplyBlurEffect(1);
            }

            if (gameEventData.Name == GameEvents.LineNarrationEnd)
                ApplyBlurEffect(0); // reset
        }
    }
}