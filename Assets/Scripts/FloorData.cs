using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "Floor")]
public class FloorData : ScriptableObject
{
    public readonly int CharlotteApartmentNumber = 1004;
    public readonly int PlayerApartmentFloor = 42;
    public readonly int PlayerApartmentNumber = 422;
    public readonly int StacyApartmentNumber = 22;
    public readonly int ZekeApartmentNumber = 503;
}