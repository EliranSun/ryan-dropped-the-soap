using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEffectsController : MonoBehaviour
{
    [SerializeField] private AudioClip levelWonSound;
    [SerializeField] private AudioClip levelLostSound;

    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void OnNotify(GameEventData gameEvent)
    {
        switch (gameEvent.name)
        {
            case GameEvents.LevelWon:
                _audioSource.PlayOneShot(levelWonSound);
                break;
            case GameEvents.LevelLost:
                _audioSource.PlayOneShot(levelLostSound);
                break;
        }
    }
}