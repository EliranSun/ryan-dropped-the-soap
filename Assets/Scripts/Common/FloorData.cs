using System;
using UnityEngine;

namespace Common
{
    [Serializable]
    [CreateAssetMenu(fileName = "Data", menuName = "Floor")]
    public class FloorData : ScriptableObject
    {
        [SerializeField] public int currentFloorNumber = 20;
    }
}