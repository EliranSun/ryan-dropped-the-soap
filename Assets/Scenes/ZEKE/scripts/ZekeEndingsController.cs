using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes.ZEKE.scripts
{
    public class ZekeEndingsController : MonoBehaviour
    {
        [SerializeField] private GameObject zekeShouts;
        [SerializeField] private GameObject zekeSuicide;
        [SerializeField] private GameObject zekeBossStub;

        private bool _isZekeShoutsActive;

        private void Awake()
        {
            if (zekeSuicide) zekeSuicide.SetActive(false);
            if (zekeShouts) zekeShouts.SetActive(false);
            if (zekeBossStub) zekeBossStub.SetActive(false);
        }

        public void OnNotify(GameEventData gameEventData)
        {
            switch (gameEventData.Name)
            {
                case GameEvents.TriggerZekeShout when !_isZekeShoutsActive:
                {
                    if (zekeShouts) zekeShouts.SetActive(true);
                    _isZekeShoutsActive = true;
                    zekeShouts.GetComponent<ZekeShoutsController>().Init();
                    break;
                }
                case GameEvents.TriggerZekeBossStub:
                {
                    if (zekeBossStub) zekeBossStub.SetActive(true);
                    break;
                }

                case GameEvents.EndZekeShouts:
                    zekeShouts.SetActive(false);
                    _isZekeShoutsActive = false;
                    SceneManager.LoadScene(SceneManager.GetSceneByName("0. The beach").ToString());
                    break;

                case GameEvents.TriggerZekeSuicide:
                {
                    if (zekeSuicide) zekeSuicide.SetActive(true);
                    break;
                }
            }
        }
    }
}