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
        private Coroutine _doorCoroutine;
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
            if (distance < distanceToOpen && !_doorsAreOpen)
                _doorCoroutine ??= StartCoroutine(OpenDoorsWithDelay());
            else if (distance >= distanceToOpen && _doorsAreOpen)
                _doorCoroutine ??= StartCoroutine(CloseDoorsWithDelay());
        }

        private IEnumerator OpenDoorsWithDelay()
        {
            yield return new WaitForSeconds(delayBeforeOpening);

            _closeLerpTime = 0;

            while (_openLefpTime <= 1)
            {
                _openLefpTime += Time.deltaTime / timeToOpen;
                doorsTransform.position =
                    Vector3.Lerp(doorsTransform.position, doorsOpenPosition.position, _openLefpTime);
                yield return null;
            }

            _doorsAreOpen = true;
            _doorCoroutine = null;
        }

        private IEnumerator CloseDoorsWithDelay()
        {
            yield return new WaitForSeconds(delayBeforeClosing);

            _openLefpTime = 0;
            while (_closeLerpTime <= 1)
            {
                _closeLerpTime += Time.deltaTime / timeToOpen;
                doorsTransform.position = Vector3.Lerp(doorsTransform.position, _initialPosition, _closeLerpTime);
                yield return null;
            }

            _doorsAreOpen = false;
            _doorCoroutine = null;
        }
    }
}