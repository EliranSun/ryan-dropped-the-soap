using UnityEngine;

namespace Player
{
    public class PlayerApartmentController : ObserverSubject
    {
        [SerializeField] private GameObject plant;
        [SerializeField] private GameObject painting;

        [SerializeField] private GameObject mirror;

        // TODO: DRY
        public readonly int ApartmentNumber = 420;

        private void Start()
        {
            // PositionItem(plant, "PlayerPlacePlantPosition");
            // PositionItem(painting, "PlayerPlacePaintingPosition");
            // PositionItem(mirror, "PlayerPlaceMirrorPosition");
        }

        private static void PositionItem(GameObject item, string key)
        {
            var storedPosition = PlayerPrefs.GetString(key);
            if (storedPosition == "")
                item.SetActive(false);

            var positionParts = storedPosition.Split(',');

            if (
                positionParts.Length == 2 &&
                float.TryParse(positionParts[0], out var x) &&
                float.TryParse(positionParts[1], out var y)
            )
                item.transform.position = new Vector2(x, y);
        }
    }
}