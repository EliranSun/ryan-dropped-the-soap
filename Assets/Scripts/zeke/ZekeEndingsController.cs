using Scenes.ZEKE.scripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace zeke
{
    public class ZekeEndingsController : MonoBehaviour
    {
        [SerializeField] private GameObject zekeShouts;
        [SerializeField] private GameObject zekeSuicide;
        [SerializeField] private GameObject zekeBossStub;
        [SerializeField] private AudioSource bgAudioSource;
        [SerializeField] private SceneAsset nextScene;

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


                case GameEvents.TriggerZekeSuicide:
                {
                    if (zekeSuicide)
                    {
                        bgAudioSource.Stop();
                        zekeSuicide.SetActive(true);
                    }

                    break;
                }
                case GameEvents.EndZekeShouts:
                    PlayerPrefs.SetString("Zeke Scene End", "Shout");
                    zekeShouts.SetActive(false);
                    _isZekeShoutsActive = false;
                    SceneManager.LoadScene("apartment scene - zeke");
                    break;

                case GameEvents.ZekeSuicideEnd:
                    PlayerPrefs.SetString("Zeke Scene End", "Suicide");
                    zekeSuicide.SetActive(false);
                    SceneManager.LoadScene("apartment scene - zeke");
                    break;
            }
        }
    }
}