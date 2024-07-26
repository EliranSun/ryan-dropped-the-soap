using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioClip levelWonSound;
    [SerializeField] private AudioClip levelLostSound;
    [SerializeField] private AudioSource bgMusicAudioSource;
    [SerializeField] private AudioSource soundEffectsAudioSource;

    public void OnNotify(GameEventData gameEvent)
    {
        switch (gameEvent.name)
        {
            case GameEvents.LevelWon:
                soundEffectsAudioSource.PlayOneShot(levelWonSound);
                bgMusicAudioSource.Stop();
                break;

            case GameEvents.LevelLost:
                soundEffectsAudioSource.PlayOneShot(levelLostSound);
                bgMusicAudioSource.Stop();
                break;
        }
    }
}