using UnityEngine;

namespace Mini_Games.Organize_Desk.scripts
{
    [RequireComponent(typeof(Collider2D))]
    public class ScoreableItem : MonoBehaviour
    {
        [SerializeField] public int score;
        public bool isThrown;

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Desk")) isThrown = false;
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Desk")) isThrown = true;
        }
    }
}