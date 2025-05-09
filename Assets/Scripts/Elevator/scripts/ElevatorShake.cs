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
                Vector2 shakeOffset = Random.insideUnitCircle * shakeAmount;
                transform.localPosition = new Vector3(
                    _originalPos.x + shakeOffset.x,
                    _originalPos.y + shakeOffset.y,
                    _originalPos.z // preserve original Z
                );
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