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
            if (_isZekeSceneEnded)
            {
                var randomPainting = paintings[Random.Range(0, paintings.Length)];
                randomPainting.gameObject.transform.parent = player.transform;
                randomPainting.transform.localPosition = new Vector3(0, 1.5f, 0);
                randomPainting.transform.localRotation = Quaternion.identity;
                randomPainting.GetComponent<SpriteRenderer>().sortingOrder = 10;
            }
        }
    }
}