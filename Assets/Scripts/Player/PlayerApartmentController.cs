using UnityEngine;

namespace Player
{
    public class PlayerApartmentController : MonoBehaviour
    {
        // TODO: DRY
        [SerializeField] private GameObject plant;
        [SerializeField] private GameObject painting;
        [SerializeField] private GameObject mirror;

        private void Start()
        {
            PositionItem(plant, "PlayerPlacePlantPosition");
            PositionItem(painting, "PlayerPlacePaintingPosition");
            PositionItem(mirror, "PlayerPlaceMirrorPosition");
        }

        public void OnNotify(GameEventData eventData)
        {
            switch (eventData.Name)
            {
                case GameEvents.PlayerPlacePlant:
                case GameEvents.PlayerPlaceMirror:
                case GameEvents.PlayerPlacePainting:
                {
                    var itemPosition = (Vector3)eventData.Data;
                    PlayerPrefs.SetString($"{eventData.Name}Position", $"{itemPosition.x},{itemPosition.y}");
                    break;
                }
            }
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
                item.transform.position = new Vector2(x, -1.5f);
        }
    }
}