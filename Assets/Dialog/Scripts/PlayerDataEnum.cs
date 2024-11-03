using System;

namespace Dialog.Scripts
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

        // Painting
        CompositionEight,
        WandererAboveTheSeaOfFog,

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