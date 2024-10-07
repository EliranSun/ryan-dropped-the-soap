using System;
using UnityEngine;

namespace dialog.scripts
{
    public enum ChoiceType
    {
        Button,
        InconsequentialButton,
        TextInput
    }

    [Serializable]
    public class PlayerChoice
    {
        public string text;
        public ChoiceType type;
        public DialogueLineObject next;
        public PlayerDataEnum choiceDataType;
    }

    [Serializable]
    public class VoicedLine
    {
        public string text;
        public AudioClip clip;
        public CharacterType gender;
    }

    [Serializable]
    public enum DialogReactions
    {
        None,
        Happy,
        Sad,
        Anger,
        Fear
    }

    [Serializable]
    public class PlayerReactions
    {
        public DialogReactions reaction;
        public DialogueLineObject line;
    }

    [CreateAssetMenu(fileName = "NarrationLine", menuName = "Dialogue/Narration Line")]
    public class DialogueLineObject : ScriptableObject
    {
        // to dynamically replace the line. Replacing the actual line will be saved
        // and we do not want that mainly because of restarting and changing information
        // TODO: I think playerName is deprecated - we replace only in the API and visually without 
        // actually altering the data here
        public string playerName;
        public VoicedLine[] voicedLines;
        public PlayerChoice[] playerOptions;
        public PlayerReactions[] playerReactions;
        public DialogueLineObject nextDialogueLine;
        public float wait = 0.5f;
        public GameEvents actionAfterLine;
    }
}