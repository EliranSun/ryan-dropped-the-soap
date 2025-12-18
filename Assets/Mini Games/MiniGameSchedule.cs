using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mini_Games
{
    [Serializable]
    public class ScheduledMiniGame
    {
        public int hour;
        public int minute;
        public MiniGameName miniGameName;
        public GameObject miniGame;
    }

    [CreateAssetMenu(menuName = "Game/Daily MiniGame Schedule")]
    public class MiniGameSchedule : ScriptableObject
    {
        public List<ScheduledMiniGame> entries;
    }
}