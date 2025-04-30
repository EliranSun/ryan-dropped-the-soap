using UnityEngine;
using UnityEngine.U2D;

namespace Scenes.Stacy.scripts
{
    [RequireComponent(typeof(Light2DBase))]
    public class ToggleLight : MonoBehaviour
    {
        [SerializeField] private KeyCode toggleKey = KeyCode.F;
        [SerializeField] private Light2DBase light;
        private bool _isDisabled;

        private void Update()
        {
            if (_isDisabled) return;

            if (Input.GetKeyDown(toggleKey)) light.enabled = !light.enabled;
        }

        public void DisableLightToggle()
        {
            _isDisabled = true;
            light.enabled = false;
        }
    }
}