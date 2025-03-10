using UnityEngine;

namespace Mini_Games.Organize_Desk.scripts
{
    public class OnCollisionGameEvent : ObserverSubject
    {
        [SerializeField] private string tagToDetect;
        [SerializeField] private GameEvents gameEventName;

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag(tagToDetect))
                Notify(gameEventName);
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.CompareTag(tagToDetect))
                Notify(gameEventName);
        }
    }
}