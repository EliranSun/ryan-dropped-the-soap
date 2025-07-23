using System.Linq;
using Dialog;
using UnityEngine;

namespace Player
{
    public class ZekeApartmentController : ObserverSubject
    {
        [SerializeField] private Transform player;
        [SerializeField] private NarrationDialogLine playerNeedPaintingLine;
        [SerializeField] private NarrationDialogLine sceneEndedAliveLine;
        [SerializeField] private NarrationDialogLine sceneEndedDeadLine;
        [SerializeField] private GameObject[] paintings;
        [SerializeField] private GameObject[] playerPaintings;
        private bool _isZekeSceneEnded;

        private void Start()
        {
            PlayerPrefs.SetString("Zeke Scene End", "Shout");

            var zekeSceneOutcome = PlayerPrefs.GetString("Zeke Scene End", "");
            _isZekeSceneEnded = zekeSceneOutcome != "";
            var storedPainting = PlayerPrefs.GetString("PlayerHoldingPainting", "");

            print($"ZEKE SCENE OUTCOME {zekeSceneOutcome}");

            if (storedPainting != "")
                // if player already has painting - this sequence have been played already
                return;

            if (_isZekeSceneEnded)
                Notify(GameEvents.TriggerSpecificDialogLine,
                    zekeSceneOutcome == "Shout" ? sceneEndedAliveLine : sceneEndedDeadLine);

            var painting = paintings[Random.Range(0, paintings.Length)];
            painting.gameObject.transform.parent = player.transform;
            painting.transform.localPosition = new Vector3(0, 2.5f, 0);
            painting.transform.localRotation = Quaternion.identity;
            painting.GetComponent<SpriteRenderer>().sortingOrder = 10;
            PlayerPrefs.SetString("PlayerHoldingPainting", painting.name);
        }

        public void OnNotify(GameEventData gameEvent)
        {
            if (gameEvent.Name.ToString().StartsWith("PaintingChosen"))
            {
                var paintingName = gameEvent.Name.ToString().Split("PaintingChosen").Last();
                var wallPainting = paintings.First(x => x.name == paintingName);
                var playerPainting = playerPaintings.First(x => x.name == paintingName);

                wallPainting.gameObject.SetActive(false);
                playerPainting.gameObject.SetActive(true);

                PlayerPrefs.SetString("PlayerHoldingPainting", wallPainting.name);
            }
        }
    }
}