using UnityEngine;

namespace Scenes.ZEKE.scripts
{
    public class BossOfficeController : MonoBehaviour
    {
        [SerializeField] private GameObject zekeShouts;
        [SerializeField] private GameObject zekeBossStub;

        private bool _isZekeShoutsActive;

        private void Awake()
        {
            if (zekeShouts) zekeShouts.SetActive(false);
            if (zekeBossStub) zekeBossStub.SetActive(false);
        }

        public void OnNotify(GameEventData gameEventData)
        {
            if (gameEventData.Name == GameEvents.TriggerZekeShout && !_isZekeShoutsActive)
            {
                if (zekeShouts) zekeShouts.SetActive(true);
                _isZekeShoutsActive = true;
                zekeShouts.GetComponent<ZekeShoutsController>().Init();
            }

            if (gameEventData.Name == GameEvents.TriggerZekeBossStub)
                if (zekeBossStub)
                    zekeBossStub.SetActive(true);
        }
    }
}