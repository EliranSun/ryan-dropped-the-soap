using System;
using UnityEngine;

namespace Common
{
    [Serializable]
    [CreateAssetMenu(fileName = "Data", menuName = "Floor")]
    public class FloorData : ScriptableObject
    {
        [SerializeField] public int playerFloorNumber = 20;
        [SerializeField] public int elevatorFloorNumber = 20;
        public bool playerExitElevator;
        public int charlotteInitFloorNumber = 42;
        public bool zekeStoryDone;
        public bool stacyStoryDone;
        public readonly int playerApartmentFloor = 42;
        public readonly int playerApartmentNumber = 420;
        public readonly int stacyFloorNumber = 38;
        public readonly int zekeFloorNumber = 50;
    }
}