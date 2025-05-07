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
    }
}