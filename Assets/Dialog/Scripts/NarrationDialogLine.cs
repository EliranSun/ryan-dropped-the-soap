using System;
using UnityEngine;

namespace Dialog.Scripts
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
        public PlayerDataEnum choiceDataType;
        public NarrationDialogLine next;
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
        public NarrationDialogLine line;
    }

    [Serializable]
    public enum InteractableObjectType
    {
        Unknown,
        Painting,
        Mirror,
        Door,
        Vase,
        Chair
    }


    [Serializable]
    public enum InteractableObjectName
    {
        Unknown,

        PaintingTheKiss,
        PaintingStarryNight,
        PaintingImpression,
        PaintingAngelus,
        PaintingComposition,
        PaintingNighthawks,
        PaintingWanderer,

        MirrorRoman,
        MirrorGreek,
        MirrorEgyptian,
        MirrorSameritan,

        DoorBlue,
        DoorBlack,
        DoorGreen,
        DoorWhite,
        DoorRed,
        DoorYellow,

        VaseEgyptian,
        VaseGreek,
        VaseJapanese,
        VaseModern,
        VasePersian,
        VaseRoman,
        VaseSameritan,

        ChairRoman,
        ChairModern,
        ChairJapanese,
        ChairGreek,
        ChairEgyptian,
        ChairSameritan
    }

    [Serializable]
    public class ObjectReferringLine
    {
        public NarrationDialogLine line;
        public InteractableObjectName objectName;
        public InteractableObjectType objectType;
    }

    [Serializable]
    [CreateAssetMenu(fileName = "NarrationLine", menuName = "Line")]
    public class NarrationDialogLine : ScriptableObject
    {
        // to dynamically replace the line. Replacing the actual line will be saved
        // and we do not want that mainly because of restarting and changing information
        // TODO: I think playerName is deprecated - we replace only in the API and visually without 
        // actually altering the data here
        public string playerName;
        public VoicedLine[] voicedLines;
        public PlayerChoice[] playerOptions;
        public PlayerReactions[] playerReactions;
        public NarrationDialogLine nextDialogueLine;
        public NarrationDialogLine[] randomizedDialogLines;
        public ObjectReferringLine[] objectReferringDialogLines;
        public float wait = 0.5f;
        public ActorName actorName;
        public GameEvents actionAfterLine;
    }
}