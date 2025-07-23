using System;
using System.Linq;
using Dialog;
using UnityEngine;

namespace Player
{
    [Serializable]
    public class Scenario
    {
        public string name;
        public NarrationDialogLine line;
    }

    public class NpcApartmentController : ObserverSubject
    {
        [SerializeField] private string sceneEndKey;
        [SerializeField] private string itemKey;
        [SerializeField] private Transform player;
        [SerializeField] private NarrationDialogLine preSceneInitLine;
        [SerializeField] private Scenario[] postSceneScenarios;
        [SerializeField] private GameObject[] items;
        [SerializeField] private GameObject[] playerItems;
        private bool _isSceneEnded;

        private void Start()
        {
            var sceneOutcome = PlayerPrefs.GetString(sceneEndKey, "");
            _isSceneEnded = sceneOutcome != "";
            var hasItem = PlayerPrefs.GetInt(itemKey, 0);

            print($"SCENE OUTCOME {sceneOutcome}");

            if (hasItem != 0)
            {
                // the player already has an item
                var apartmentItem = items.First(x => x.name == itemKey);
                apartmentItem.gameObject.SetActive(false);
                return;
            }

            if (_isSceneEnded)
            {
                var scenario = postSceneScenarios.First(scenario => scenario.name == sceneOutcome);
                Notify(GameEvents.TriggerSpecificDialogLine, scenario.line);
            }

            // var painting = items[Random.Range(0, items.Length)];
            // painting.gameObject.transform.parent = player.transform;
            // painting.transform.localPosition = new Vector3(0, 2.5f, 0);
            // painting.transform.localRotation = Quaternion.identity;
            // painting.GetComponent<SpriteRenderer>().sortingOrder = 10;
            // PlayerPrefs.SetString("PlayerHoldingItem", painting.name);
        }

        public void OnNotify(GameEventData gameEvent)
        {
            var chosenItemKey = $"{itemKey}Chosen";
            if (gameEvent.Name.ToString().StartsWith(chosenItemKey))
            {
                var objectName = gameEvent.Name.ToString().Split(chosenItemKey).Last();
                var apartmentItem = items.First(x => x.name == objectName);
                var playerItem = playerItems.First(x => x.name == objectName);

                apartmentItem.gameObject.SetActive(false);
                playerItem.gameObject.SetActive(true);

                PlayerPrefs.SetString("PlayerHoldingItem", apartmentItem.name);
                PlayerPrefs.SetInt(itemKey, 1);
            }
        }
    }
}