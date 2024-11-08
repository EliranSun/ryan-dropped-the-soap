using System;
using Dialog.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace museum_dialog.scripts
{
    [Serializable]
    public class DialogueState
    {
        [FormerlySerializedAs("playerDataProperty")]
        public PlayerDataEnum missingPlayerData;

        [FormerlySerializedAs("nextState")] public NarrationDialogLine dialogLineToPlay;
    }

    public class DialogueStateChanger : MonoBehaviour
    {
        [SerializeField] private bool isDisabled;

        [FormerlySerializedAs("prioritizedStates")] [FormerlySerializedAs("states")] [SerializeField]
        private DialogueState[] requiredStatesForScene;

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
            if (isDisabled) return ScriptableObject.CreateInstance<NarrationDialogLine>();

            if (requiredStatesForScene.Length == 0)
                return finalState.dialogLineToPlay;

            foreach (var state in requiredStatesForScene)
            {
                var savedProperty = PlayerPrefs.GetString(state.missingPlayerData.ToString()).Trim();

                if (string.IsNullOrEmpty(savedProperty) || savedProperty.Trim().Length <= 1)
                    return state.dialogLineToPlay;
            }

            return finalState.dialogLineToPlay;
        }
    }
}