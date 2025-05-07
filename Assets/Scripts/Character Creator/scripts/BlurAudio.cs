using Dialog;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BlurAudio : MonoBehaviour
{
    [SerializeField] private float _defaultCutoffFrequency = 22000f; // Default cutoff (no filter)
    [SerializeField] private float _minCutoffFrequency = 400f; // Heavy muffling
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
            _lowPassFilter.cutoffFrequency = _defaultCutoffFrequency;
        }
    }

    /// <summary>
    ///     Applies a muffled effect to the audio based on the blur level
    /// </summary>
    /// <param name="blurLevel">0 = no blur, 1 = maximum blur</param>
    public void ApplyBlurEffect(float blurLevel)
    {
        // Clamp blur level between 0 and 1
        blurLevel = Mathf.Clamp01(blurLevel);

        if (blurLevel <= 0)
        {
            // No blur - set to default (high) cutoff frequency
            _lowPassFilter.cutoffFrequency = _defaultCutoffFrequency;
        }
        else
        {
            // Calculate cutoff frequency based on blur level
            // As blur increases, cutoff frequency decreases (more muffled)
            var cutoffFrequency = Mathf.Lerp(_defaultCutoffFrequency, _minCutoffFrequency, blurLevel);
            _lowPassFilter.cutoffFrequency = cutoffFrequency;
        }
    }

    /// <summary>
    ///     Apply blur effect from a NarrationDialogLine
    /// </summary>
    /// <param name="dialogLine">The dialog line containing blur level</param>
    public void ApplyBlurFromDialogLine(NarrationDialogLine dialogLine)
    {
        if (dialogLine != null) ApplyBlurEffect(dialogLine.angerLevel);
    }

    public void OnNotify(GameEventData gameEventData)
    {
        if (gameEventData.Name == GameEvents.LineNarrationStart)
        {
            var narrationDialogLine = gameEventData.Data as NarrationDialogLine;
            if (narrationDialogLine == null) return;

            print("angerLevel: " + narrationDialogLine.angerLevel);

            ApplyBlurFromDialogLine(narrationDialogLine);
        }
    }
}