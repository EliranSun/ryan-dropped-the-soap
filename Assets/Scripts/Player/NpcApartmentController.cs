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
        private bool _isSceneEnded;

        private void Start()
        {
            var sceneOutcome = PlayerPrefs.GetString(sceneEndKey, "");
            _isSceneEnded = sceneOutcome != "";
            var hasItem = PlayerPrefs.GetInt(itemKey, 0);

            print($"SCENE OUTCOME {sceneOutcome}");
            var scenario = postSceneScenarios.FirstOrDefault(scenario => 
                scenario.name == sceneOutcome);

            if (scenario != null)
            {
                Notify(GameEvents.TriggerSpecificDialogLine, scenario.line);
                return;
            }

            Notify(GameEvents.TriggerSpecificDialogLine, preSceneInitLine);
        }
    }
}