using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioClip levelWonSound;
    [SerializeField] private AudioClip levelLostSound;
    [SerializeField] private AudioClip showerSound;
    [SerializeField] private AudioClip knockSoundEffect;
    [SerializeField] private AudioSource bgMusicAudioSource;
    [SerializeField] private AudioSource soundEffectsAudioSource;


    public void OnNotify(GameEventData gameEvent)
    {
        switch (gameEvent.name)
        {
            case GameEvents.LevelWon:
                soundEffectsAudioSource.PlayOneShot(levelWonSound);
                bgMusicAudioSource.Stop();
                Invoke(nameof(StopSoundEffect), 1);
                Invoke(nameof(Knock), 3);
                break;

            case GameEvents.LevelLost:
                soundEffectsAudioSource.PlayOneShot(levelLostSound);
                bgMusicAudioSource.Stop();
                Invoke(nameof(StopSoundEffect), 1);
                Invoke(nameof(Knock), 3);
                break;

            case GameEvents.FaucetOpening:
                soundEffectsAudioSource.PlayOneShot(showerSound);
                break;

            case GameEvents.FaucetClosed:
                soundEffectsAudioSource.Stop();
                break;
        }
    }

    private void StopSoundEffect()
    {
        soundEffectsAudioSource.Stop();
    }

    private void Knock()
    {
        soundEffectsAudioSource.PlayOneShot(knockSoundEffect);
    }
}