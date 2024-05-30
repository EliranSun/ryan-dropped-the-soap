using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Ryan.Scripts {
    [RequireComponent(typeof(Scrollbar))]
    public class SlipperinessTracker : MonoBehaviour {
        [SerializeField] private GameObject target;
        private bool _isHolding;
        private Scrollbar _scrollbar;
        private Rigidbody2D _targetRigidBody;

        private void Start() {
            _targetRigidBody = target.GetComponent<Rigidbody2D>();
            _scrollbar = GetComponent<Scrollbar>();
        }

        private void Update() {
            if (_isHolding)
                return;

            var zRotation = (float)Math.Round(target.transform.rotation.z, 2);
            zRotation = Mathf.Clamp(zRotation, -0.5f, 0.5f);

            _scrollbar.value = 1.0f - (zRotation + 0.5f) / 1.0f;
        }

        public void OnPointerDown(BaseEventData foo) {
            _isHolding = true;
        }

        public void OnPointerUp(BaseEventData foo) {
            _isHolding = false;
        }

        public void OnValueChange(float scrollbarValue) {
            if (!_isHolding)
                return;

            _targetRigidBody.velocity = Vector2.zero;
            _targetRigidBody.angularVelocity = 0f;

            var zRotation = (1.0f - scrollbarValue) * 1.0f - 0.5f;
            var currentRotation = target.transform.rotation.eulerAngles;
            currentRotation.z = zRotation * 180.0f; // Convert to degrees
            target.transform.rotation = Quaternion.Euler(currentRotation);
        }

        public void OnNotify(GameEventData eventData) {
            if (eventData.name == GameEvents.PlayerChangeState)
                target = (GameObject)eventData.data;
        }
    }
}