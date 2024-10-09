using System;
using dialog.scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace museum_dialog.scripts
{
    [Serializable]
    public class DialogueState
    {
        public PlayerDataEnum playerDataProperty;
        public NarrationDialogLine nextState;
    }

    public class DialogueStateChanger : MonoBehaviour
    {
        [FormerlySerializedAs("states")] [SerializeField]
        private DialogueState[] prioritizedStates;

        [SerializeField] private DialogueState finalState; // if all other states are met
        public static DialogueStateChanger Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;
        }

        public NarrationDialogLine GetDialogStateByPlayerPrefs()
        {
            if (prioritizedStates.Length == 0)
                return finalState.nextState;

            foreach (var state in prioritizedStates)
            {
                var savedProperty = PlayerPrefs.GetString(state.playerDataProperty.ToString()).Trim();

                if (string.IsNullOrEmpty(savedProperty) || savedProperty.Trim().Length <= 1)
                    return state.nextState;
            }

            return finalState.nextState;
        }
    }
}