using System.Collections;
using UnityEngine;

namespace Elevator.scripts
{
    public class ElevatorDoorsController : MonoBehaviour
    {
        [SerializeField] private Transform doorsOpenPosition;
        [SerializeField] private Transform doorsTransform;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float distanceToOpen = 1.5f;
        [SerializeField] private float timeToOpen = 1.5f;
        [SerializeField] private float delayBeforeOpening = 2f;
        [SerializeField] private float delayBeforeClosing = 2f;

        private float _closeLerpTime;
        private bool _doorCloseCoroutineStarted;
        private bool _doorOpenCoroutineStarted;
        private bool _doorsAreOpen;
        private Vector3 _initialPosition;
        private float _openLefpTime;

        private void Start()
        {
            _initialPosition = doorsTransform.position;
            _doorsAreOpen = false;
        }

        private void Update()
        {
            var distance = Vector3.Distance(playerTransform.position, transform.position);

            if (distance < distanceToOpen && !_doorsAreOpen && !_doorOpenCoroutineStarted)
            {
                StartCoroutine(OpenDoorsWithDelay());
                _doorOpenCoroutineStarted = true;
            }

            if (distance >= distanceToOpen && _doorsAreOpen && !_doorCloseCoroutineStarted)
            {
                StartCoroutine(CloseDoorsWithDelay());
                _doorCloseCoroutineStarted = false;
            }
        }

        private IEnumerator OpenDoorsWithDelay()
        {
            yield return new WaitForSeconds(delayBeforeOpening);

            _openLefpTime = 0;
            _closeLerpTime = 0;

            while (doorsTransform.position != doorsOpenPosition.position)
            {
                _openLefpTime += Time.deltaTime / timeToOpen;
                print($"_openLefpTime: {_openLefpTime}");
                doorsTransform.position =
                    Vector3.Lerp(doorsTransform.position, doorsOpenPosition.position, _openLefpTime);
                yield return null;
            }

            _doorsAreOpen = true;
            _doorOpenCoroutineStarted = false;
            _doorCloseCoroutineStarted = false;
        }

        private IEnumerator CloseDoorsWithDelay()
        {
            yield return new WaitForSeconds(delayBeforeClosing);

            _openLefpTime = 0;
            _closeLerpTime = 0;

            while (doorsTransform.position != _initialPosition)
            {
                _closeLerpTime += Time.deltaTime / timeToOpen;
                doorsTransform.position = Vector3.Lerp(doorsTransform.position, _initialPosition, _closeLerpTime);
                yield return null;
            }

            _doorsAreOpen = false;
            _doorCloseCoroutineStarted = false;
            _doorOpenCoroutineStarted = false;
        }
    }
}