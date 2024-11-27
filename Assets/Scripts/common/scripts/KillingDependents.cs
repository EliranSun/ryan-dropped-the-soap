using UnityEngine;

namespace common.scripts
{
    public class KillingDependents : MonoBehaviour
    {
        [SerializeField] private GameObject[] dependents;
        [SerializeField] private Texture2D bubbleCursorTexture;
        private GameObject favouriteDependent;

        private void Start()
        {
            foreach (var dependent in dependents)
                dependent.SetActive(false);
        }

        public void ActivateDependents()
        {
            foreach (var dependent in dependents)
            {
                if (dependent == favouriteDependent)
                    continue;

                dependent.SetActive(true);
            }

            Cursor.SetCursor(bubbleCursorTexture, Vector2.zero, CursorMode.Auto);
        }

        public void SetFavouriteDependent(GameObject dependent)
        {
            favouriteDependent = dependent;
        }
    }
}