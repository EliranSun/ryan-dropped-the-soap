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
            PositionPlant();
            // PositionPainting();
            // PositionMirror();
        }

        public void OnNotify(GameEventData data)
        {
        }

        // TODO: DRY
        private void PositionPainting()
        {
            var storedPaintingPosition = PlayerPrefs.GetString("PlayerPaintingPosition");
            if (storedPaintingPosition == "")
                painting.SetActive(false);
        }

        private void PositionMirror()
        {
            var storedMirrorPosition = PlayerPrefs.GetString("PlayerMirrorPosition");
            if (storedMirrorPosition == "")
                mirror.SetActive(false);
        }

        private void PositionPlant()
        {
            var storedPlantPosition = PlayerPrefs.GetString("PlayerPlantPosition");
            if (storedPlantPosition == "")
                plant.SetActive(false);

            var plantPositionParts = storedPlantPosition.Split(',');

            if (
                plantPositionParts.Length == 2 &&
                float.TryParse(plantPositionParts[0], out var x) &&
                float.TryParse(plantPositionParts[1], out var y)
            )
                plant.transform.position = new Vector2(x, -1.5f);
        }
    }
}