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


            switch (data.Name)
            {
                case GameEvents.PlayerPlacePlant:
                {
                    if (PlayerPrefs.GetInt("HeardCharlottePlantInstructions") != 1)
                        return;

                    Grow();
                    break;
                }

                case GameEvents.PlayerPlaceMirror:
                {
                    if (PlayerPrefs.GetInt("HeardCharlottePlantInstructions") != 1)
                        return;

                    Grow();
                    break;
                }

                case GameEvents.PlayerPlacePainting:
                {
                    if (PlayerPrefs.GetInt("HeardCharlottePlantInstructions") != 1)
                        return;

                    Grow();
                    break;
                }
            }
        }

        private void Grow()
        {
            level++;
            PlayerPrefs.SetInt("PlayerGrowth", level);
            UpdateBodies();
        }

        private void UpdateBodies()
        {
            print($"Current growth level {level}");

            for (var i = 0; i < bodies.Length; i++)
            {
                print($"Setting {bodies[i].name} to {level == i}");
                bodies[i].SetActive(level == i);
            }

            if (level > 0)
                Notify(GameEvents.PlayerGrew, bodies[level]);
        }
    }
}