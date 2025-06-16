using UnityEngine;

namespace Player
{
    public class PlayerGrowth : ObserverSubject
    {
        [SerializeField] private GameObject starterBody;
        [SerializeField] private GameObject levelOneBody;
        [SerializeField] private Movement movement;
        [SerializeField] private PlayerScriptableObject playerData;
        private int _level;

        private void Start()
        {
            _level = playerData.playerGrowth;
            starterBody.SetActive(_level == 0);
            levelOneBody.SetActive(_level == 1);
        }

        public void OnNotify(GameEventData data)
        {
            if (data.Name == GameEvents.PlayerPlacePlant &&
                playerData.heardCharlottePlantInstructions)
            {
                _level++;
                playerData.playerGrowth = _level;
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