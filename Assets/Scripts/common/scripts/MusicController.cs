using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace common.scripts
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicController : MonoBehaviour
    {
        private AudioSource audioSource;
        [SerializeField] private float _delay;
        [SerializeField] private float _changeSpeedFactor;
        [SerializeField] private float _minPitch;
        [SerializeField] private float _maxPitch;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            audioSource.Stop();
            Invoke(nameof(PlayMusic), _delay);
        }

        private void PlayMusic()
        {
            audioSource.time = 0;
            audioSource.Play();
        }

        public void OnNotify(GameEventData eventData)
        {
            switch (eventData.Name)
            {
                case GameEvents.SlowDownMusic:
                    audioSource.pitch = Mathf.Max(audioSource.pitch - _changeSpeedFactor, _minPitch);
                    break;

                case GameEvents.SpeedUpMusic:
                    audioSource.pitch = Mathf.Min(audioSource.pitch + _changeSpeedFactor, _maxPitch);
                    break;

                case GameEvents.StopMusic:
                    audioSource.Stop();
                    break;

                case GameEvents.StartMusic:
                    audioSource.Play();
                    break;
            }
        }
    }
}