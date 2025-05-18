using UnityEngine;

namespace Common.scripts
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEffectsController : MonoBehaviour
    {
        private AudioSource _audioSource;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void OnNotify(GameEventData gameEvent)
        {
            if (gameEvent.Name == GameEvents.TriggerSoundEffect)
            {
                var soundEffect = gameEvent.Data as AudioClip;
                _audioSource.PlayOneShot(soundEffect);
            }
        }

        public void PlaySoundEffect(AudioClip soundEffect)
        {
            _audioSource.PlayOneShot(soundEffect);
        }
    }
}