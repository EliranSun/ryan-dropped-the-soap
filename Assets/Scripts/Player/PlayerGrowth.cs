using UnityEngine;

namespace Player
{
    public class PlayerGrowth : ObserverSubject
    {
        [SerializeField] private int level;
        [SerializeField] private bool resetPlayerGrowth;
        [SerializeField] private bool testMode;
        [SerializeField] private GameObject[] bodies;

        private void Awake()
        {
            if (resetPlayerGrowth)
                PlayerPrefs.SetInt("PlayerGrowth", 0);
        }

        private void Start()
        {
            if (!testMode) level = PlayerPrefs.GetInt("PlayerGrowth");
            UpdateBodies();
        }

        public void OnNotify(GameEventData data)
        {
            if (testMode) return;

            if (PlayerPrefs.GetInt("HeardCharlottePlantInstructions") != 1)
                return;

            switch (data.Name)
            {
                case GameEvents.PlayerPlacePlant:
                case GameEvents.PlayerPlaceMirror:
                case GameEvents.PlayerPlacePainting:
                {
                    Grow();
                    break;
                }
            }
        }

        private void Grow()
        {
            if (level + 1 >= bodies.Length) return;

            level++;
            PlayerPrefs.SetInt("PlayerGrowth", level);
            UpdateBodies();
        }

        private void UpdateBodies()
        {
            for (var i = 0; i < bodies.Length; i++) bodies[i].SetActive(level == i);

            if (level > 0)
                Notify(GameEvents.PlayerGrew, bodies[level]);
        }
    }
}