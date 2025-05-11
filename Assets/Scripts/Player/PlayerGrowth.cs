using Character.Scripts;
using UnityEngine;

namespace Player
{
    public class PlayerGrowth : MonoBehaviour
    {
        [SerializeField] private GameObject starterBody;
        [SerializeField] private GameObject levelOneBody;
        [SerializeField] private Movement movement;
        private int _level;

        private void Start()
        {
            starterBody.SetActive(_level == 0);
            levelOneBody.SetActive(_level == 1);
        }

        public void OnNotify(GameEventData data)
        {
            if (data.Name != GameEvents.PlayerGrowth) return;

            _level++;

            if (_level == 1)
            {
                starterBody.SetActive(false);
                levelOneBody.SetActive(true);
                movement.spriteRenderer = levelOneBody.GetComponent<SpriteRenderer>();
            }
        }
    }
}