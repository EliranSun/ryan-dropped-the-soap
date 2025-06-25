using UnityEngine;

namespace Player
{
    public class PlayerGrowth : ObserverSubject
    {
        [SerializeField] private GameObject starterBody;
        [SerializeField] private GameObject levelOneBody;
        [SerializeField] private bool resetPlayerGrowth;
        [SerializeField] private int level;

        private void Awake()
        {
            if (resetPlayerGrowth) PlayerPrefs.SetInt("PlayerGrowth", 0);
        }

        private void Start()
        {
            level = PlayerPrefs.GetInt("PlayerGrowth");
            starterBody.SetActive(level == 0);

            if (level > 0)
            {
                levelOneBody.SetActive(true);
                Notify(GameEvents.PlayerGrew, levelOneBody);
            }
        }

        public void OnNotify(GameEventData data)
        {
            var heardCharlotteInstructions = PlayerPrefs.GetInt("HeardCharlottePlantInstructions") == 1;
            if (data.Name == GameEvents.PlayerPlacePlant && heardCharlotteInstructions)
            {
                level = 1;
                PlayerPrefs.SetInt("PlayerGrowth", level);
                starterBody.SetActive(false);
                levelOneBody.SetActive(true);
                Notify(GameEvents.PlayerGrew, levelOneBody);
            }
        }
    }
}