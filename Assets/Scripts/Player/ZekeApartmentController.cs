using Dialog;
using UnityEngine;

namespace Player
{
    public class ZekeApartmentController : MonoBehaviour
    {
        [SerializeField] private NarrationDialogLine playerNeedPaintingLine;
        [SerializeField] private NarrationDialogLine sceneEndedLine;
        [SerializeField] private GameObject[] paintings;
        [SerializeField] private Transform player;
        private bool _isZekeSceneEnded;

        private void Start()
        {
            _isZekeSceneEnded = PlayerPrefs.GetString("Zeke Scene End", "") != "";
            var storedPainting = PlayerPrefs.GetString("PlayerHoldingPainting", "");

            if (storedPainting != "")
                return;

            if (_isZekeSceneEnded)
            {
                var painting = paintings[Random.Range(0, paintings.Length)];
                painting.gameObject.transform.parent = player.transform;
                painting.transform.localPosition = new Vector3(0, 2.5f, 0);
                painting.transform.localRotation = Quaternion.identity;
                painting.GetComponent<SpriteRenderer>().sortingOrder = 10;
                PlayerPrefs.SetString("PlayerHoldingPainting", painting.name);
            }
        }
    }
}