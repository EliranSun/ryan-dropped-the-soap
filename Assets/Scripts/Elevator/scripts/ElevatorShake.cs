using UnityEngine;

namespace Elevator.scripts
{
    public class ElevatorShake : MonoBehaviour
    {
        [SerializeField] private float shakeDuration = 0.5f;
        [SerializeField] private float shakeAmount = 0.7f;
        [SerializeField] private float decreaseFactor = 1.0f;
        private Vector3 _originalPos;

        private void Update()
        {
            if (shakeDuration > 0)
            {
                transform.localPosition = _originalPos + Random.insideUnitSphere * shakeAmount;
                shakeDuration -= Time.deltaTime * decreaseFactor;
            }
            else
            {
                shakeDuration = 0f;
            }
        }

        public void Shake(float duration)
        {
            _originalPos = transform.localPosition;
            shakeDuration = duration;
        }
    }
}