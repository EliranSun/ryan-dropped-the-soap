using Character_Creator.scripts;
using UnityEngine;

namespace Object.Scripts
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(AudioSource))]
    public class ClockController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer[] digitSprites;
        [SerializeField] private Sprite[] clockDigits;
        [SerializeField] private float buzzIntensity = 0.1f; // How far the clock moves
        [SerializeField] private float buzzSpeed = 20f; // How fast the clock buzzes
        [SerializeField] private float rotationAmount = 5f; // Maximum rotation angle
        [SerializeField] private bool isBuzzing;
        [SerializeField] private AudioClip alarmSound;

        private AudioSource _clockAudioSource;
        private Vector3 _startPosition;
        private Quaternion _startRotation;

        private void Start()
        {
            _startPosition = transform.position;
            _startRotation = transform.rotation;
            _clockAudioSource = GetComponent<AudioSource>();

            if (digitSprites.Length != 4)
                Debug.LogError("ClockController: digitSprites array must have 4 elements");

            digitSprites[0].sprite = clockDigits[0];
            digitSprites[1].sprite = clockDigits[0];
            digitSprites[2].sprite = clockDigits[0];
            digitSprites[3].sprite = clockDigits[0];
        }

        private void Update()
        {
            if (isBuzzing)
            {
                // Create a shaking motion using sin waves
                var xOffset = Mathf.Sin(Time.time * buzzSpeed) * buzzIntensity;
                var yOffset = Mathf.Sin((Time.time + 0.5f) * buzzSpeed) * buzzIntensity;

                // Apply position offset
                transform.position = _startPosition + new Vector3(xOffset, yOffset, 0);

                // Apply rotation
                var rotationZ = Mathf.Sin(Time.time * buzzSpeed) * rotationAmount;
                transform.rotation = _startRotation * Quaternion.Euler(0, 0, rotationZ);
            }
        }

        public void StartBuzzing()
        {
            isBuzzing = true;
            _clockAudioSource.clip = alarmSound;
            _clockAudioSource.loop = true;
            _clockAudioSource.Play();
        }

        public void StopBuzzing()
        {
            isBuzzing = false;
            transform.position = _startPosition;
            transform.rotation = _startRotation;
            _clockAudioSource.Stop();
        }

        public void OnNotify(GameEventData eventData)
        {
            switch (eventData.name)
            {
                case GameEvents.TriggerAlarmClockSound:
                    StartBuzzing();
                    break;

                case GameEvents.TriggerAlarmClockStop:
                    StopBuzzing();
                    break;

                case GameEvents.SetClockTime:
                {
                    var playerChoice = (EnrichedPlayerChoice)eventData.data;
                    var time = playerChoice.Choice;
                    SetTimeFromString(time);
                    break;
                }
            }
        }

        private void SetTimeFromString(string time)
        {
            if (time.Length != 5)
            {
                Debug.LogError("ClockController: time string must have 4 characters");
                return;
            }

            for (var i = 0; i < 4; i++)
            {
                // 06:00
                if (i == 2) continue;

                var digit = time[i];
                if (!char.IsDigit(digit))
                {
                    Debug.LogError("ClockController: time string must contain only digits");
                    return;
                }

                var intDigit = int.Parse(digit.ToString());
                digitSprites[i].sprite = clockDigits[intDigit];
            }
        }
    }
}