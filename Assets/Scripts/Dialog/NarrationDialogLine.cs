using System;
using Dialog.Scripts;
using UnityEngine;

namespace Dialog
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
        public PlayerDataOption choiceDataOption;
        public NarrationDialogLine next;
        public GameEvents actionAfterPlayerChoice;

        public PlayerChoice(string text, NarrationDialogLine dialogResponse)
        {
            this.text = text;
            next = dialogResponse;
            type = ChoiceType.Button;
        }
    }

    [Serializable]
    public class VoicedLine
    {
        public string text;
        public AudioClip clip;
        public CharacterType gender;
    }

    [Serializable]
    public enum Reaction
    {
        Neutral,
        Happy,
        Sad,
        Anger,
        Fear,
        Disgust,
        Love,
        Hate
    }

    [Serializable]
    public class EmotionalReactionLine
    {
        public Reaction reaction;
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
        Armchair,
        ApartmentDoorKnob,
        Shape,
        Dependent
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
        ChairSameritan,

        ShapeButt,
        ShapeCleavage,
        ShapePenis,
        ShapeVagina,

        DependentBaby,
        DependentDog,
        DependentCat
    }

    [Serializable]
    public class ObjectReferringLine
    {
        public NarrationDialogLine line;
        public InteractableObjectName objectName;
        public InteractableObjectType objectType;
    }

    [Serializable]
    public enum Condition
    {
        None,
        TalkedWithOldMan,
        ZekeAtBridge,
        ZekeIsDead,
        OldManIsDead,
        TreasureFound
    }

    [Serializable]
    public class LineCondition
    {
        public Condition condition;
        public bool isMet;
    }

    [Serializable]
    public class ConditionalNextLine
    {
        public string key; // PlayerPrefs
        public string value; // PlayerPrefs
        public NarrationDialogLine line;
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
        public float waitBeforeLine;
        public bool mandatoryPlayerChoice = true;

        public Sprite overlayImageSprite;

        public VoicedLine[] voicedLines;
        public NarrationDialogLine nextDialogueLine;
        public PlayerChoice[] playerOptions;
        public EmotionalReactionLine[] playerReactions;
        public ConditionalNextLine[] conditionalNextLines;
        public NarrationDialogLine[] randomizedDialogLines;
        public ObjectReferringLine[] objectReferringDialogLines;

        public float wait = 0.5f;

        public ActorName actorName;
        public GameEvents actionBeforeLine;
        public GameEvents actionAfterLine;
        public Reaction actorReaction;

        public float angerLevel;

        public NarrationDialogLine toggleLineCondition;
        public LineCondition lineCondition;

        public bool blurAudio;

        // TODO: Make it generic?
        public int actionNumberData;
    }
}