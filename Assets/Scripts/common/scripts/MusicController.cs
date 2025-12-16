using UnityEngine;

namespace common.scripts
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicController : MonoBehaviour
    {
        [SerializeField] private float _delay;
        [SerializeField] private float _changeSpeedFactor;
        [SerializeField] private float _minPitch;
        [SerializeField] private float _maxPitch;
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            _audioSource.Stop();
            Invoke(nameof(PlayMusic), _delay);
        }

        private void PlayMusic()
        {
            _audioSource.time = 0;
            _audioSource.Play();
        }

        public void OnNotify(GameEventData eventData)
        {
            switch (eventData.Name)
            {
                case GameEvents.SlowDownMusic:
                    _audioSource.pitch = Mathf.Max(_audioSource.pitch - _changeSpeedFactor, _minPitch);
                    break;

                case GameEvents.SpeedUpMusic:
                    _audioSource.pitch = Mathf.Min(_audioSource.pitch + _changeSpeedFactor, _maxPitch);
                    break;

                case GameEvents.StopMusic:
                    _audioSource.Stop();
                    break;

                case GameEvents.StartMusic:
                    _audioSource.Play();
                    break;
            }
        }
    }
}