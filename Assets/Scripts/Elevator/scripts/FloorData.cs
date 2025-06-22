using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Elevator.scripts
{
    [Serializable]
    [CreateAssetMenu(fileName = "Data", menuName = "Floor")]
    public class FloorData : ScriptableObject
    {
        [FormerlySerializedAs("playerFloorNumber")] [SerializeField]
        public int currentFloorNumber = 20;

        [SerializeField] public int elevatorFloorNumber = 20;
        public bool playerExitElevator;
        public int charlotteInitFloorNumber = 42;
        public bool zekeStoryDone;
        public bool stacyStoryDone;

        public readonly int PlayerApartmentFloor = 42;
        public readonly int PlayerApartmentNumber = 422;
        public readonly int StacyApartmentNumber = 22;
        public readonly int ZekeApartmentNumber = 503;
    }
}