using System;

namespace Dialog
{
    [Serializable]
    public enum PlayerDataEnum
    {
        // CSS intro
        None,
        Name,
        Gender,
        Feeling,
        Partner,

        // Museum choices
        Painting,
        Door,
        Vase,
        Armchair,
        Mirror,
        Shape,
        Dependent,

        // Apartment choices
        MorningWakeUpTime
    }

    [Serializable]
    public enum PlayerDataOption
    {
        // CSS intro
        None,

        // Feelings
        Ok,
        SoSo,
        Good,
        Complicated,

        // Gender,
        Female,
        Male,

        // Door
        Blue,
        Red,
        Yellow,
        Green,
        White,
        Black,

        // Painting
        WandererAboveTheSeaOfFog,
        NightHawks,
        CompositionEight,
        TheKiss,
        Angelus,
        Impression,
        StarryNight,

        // Shapes
        Butt,
        Cleavage,
        Penis,
        Vagina,

        // Armchairs
        ArmchairEgyptian,
        ArmchairGreek,
        ArmchairJapanese,
        ArmchairModern,
        ArmchairRoman,
        ArmchairSameritan,

        // Mirrors
        MirrorEgyptian,
        MirrorGreek,
        MirrorRoman,
        MirrorSameritan,

        // Vases
        VaseEgyptian,
        VaseGreek,
        VaseJapanese,
        VaseModern,
        VasePersian,
        VaseRoman,
        VaseSamerian,

        // Built in partner names
        Ryan,
        Stacy,
        Zeke,
        Valery,
        Jayden,
        Morgan,

        // Morning wake up times
        FourAM,
        SixAM,
        SevenAM,
        EightAM,
        NineAM,
        TwoPM
    }
}