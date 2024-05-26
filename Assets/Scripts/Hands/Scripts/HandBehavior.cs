using UnityEngine;

namespace Hands.Scripts {
    public class HandBehavior : MonoBehaviour {
        [SerializeField] private GameObject openHandObject;
        [SerializeField] private GameObject closedHandObject;
        [SerializeField] private GameObject grabbingHandObject;
        [SerializeField] private GameObject soap;
        [SerializeField] private Rigidbody2D soapRigidBody;

        [SerializeField] private float grabbableSoapDistance = 4;
        [SerializeField] private bool isLeftHand;

        private void Start() {
            soapRigidBody = soap.GetComponent<Rigidbody2D>();
        }

        private void Update() {
            var mouseButtonKeyCode = isLeftHand ? 0 : 1;

            if (Input.GetMouseButtonDown(mouseButtonKeyCode)) {
                if (IsSoapGrabbable()) {
                    print("GRAB!");
                    SoapManager.IsGrabbingSoap = true;
                    GrabHand();
                    return;
                }

                SoapManager.IsGrabbingSoap = false;
                print("MISSED!");
                if (IsSoapCloseBy())
                    LaunchSoapInRandomDirection();

                CloseHand();
            }

            if (Input.GetMouseButtonUp(mouseButtonKeyCode)) {
                SoapManager.IsGrabbingSoap = false;
                OpenHand();
            }
        }

        private void CloseHand() {
            openHandObject.SetActive(false);
            closedHandObject.SetActive(true);
            grabbingHandObject.SetActive(false);
        }

        private void OpenHand() {
            openHandObject.SetActive(true);
            closedHandObject.SetActive(false);
            grabbingHandObject.SetActive(false);
        }

        private void GrabHand() {
            openHandObject.SetActive(false);
            closedHandObject.SetActive(false);
            grabbingHandObject.SetActive(true);
        }

        private bool IsSoapGrabbable() {
            var distance = Vector2.Distance(transform.position, soap.transform.position);
            return distance <= grabbableSoapDistance;
        }

        private bool IsSoapCloseBy() {
            var distance = Vector2.Distance(transform.position, soap.transform.position);
            return distance <= grabbableSoapDistance + 1;
        }

        private void LaunchSoapInRandomDirection() {
            print("WHOOSH");

            var randomX = Random.Range(-5, 5);
            var randomY = Random.Range(0, 20);

            soapRigidBody.AddForce(new Vector2(randomX, randomY), ForceMode2D.Impulse);
        }
    }
}