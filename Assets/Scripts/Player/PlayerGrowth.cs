using UnityEngine;

namespace Player
{
    public class PlayerGrowth : ObserverSubject
    {
        [SerializeField] private GameObject starterBody;
        [SerializeField] private GameObject levelOneBody;
        [SerializeField] private Movement movement;
        private int _level;

        private void Start()
        {
            _level = PlayerPrefs.GetInt("PlayerGrowth");
            starterBody.SetActive(_level == 0);
            levelOneBody.SetActive(_level == 1);
        }

        public void OnNotify(GameEventData data)
        {
            if (data.Name == GameEvents.PlayerPlacePlant &&
                PlayerPrefs.GetInt("HeardCharlottePlantInstructions") == 1)
            {
                _level++;
                PlayerPrefs.SetInt("PlayerGrowth", _level);
                starterBody.SetActive(false);
                levelOneBody.SetActive(true);
                movement.spriteRenderer = levelOneBody.GetComponent<SpriteRenderer>();
                Notify(GameEvents.PlayerGrew);
            }

            // if (data.Name != GameEvents.PlayerGrowth) 
            //     return;
            //
            // _level++;
            //
            // if (_level == 1)
            // {
            // }
        }
    }
}