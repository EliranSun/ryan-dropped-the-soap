using System;
using System.Collections.Generic;
using interactions;
using UnityEngine;

namespace Mini_Games
{
    [Serializable]
    public class ScheduledMiniGame
    {
        public int hour;
        public int minute;
        public ObjectInteractionType interactionType;
        public MiniGameName miniGameName;
    }

    [CreateAssetMenu(menuName = "Mini Game/Daily MiniGame Schedule")]
    public class MiniGameSchedule : ScriptableObject
    {
        public List<ScheduledMiniGame> entries;
    }
}