using UnityEngine;

namespace Player
{
    public enum Location
    {
        PlayerApartment,
        Hallway,
        Elevator,
        BuildingFrontView
    }

    [CreateAssetMenu(
        fileName = "PlayerScriptableObject",
        menuName = "Scriptable Objects/PlayerScriptableObject"
    )]
    public class PlayerScriptableObject : ScriptableObject
    {
        public bool isPlayerInBox = true;
        public Location location = Location.BuildingFrontView;
        public Vector2 position;
        public int playerGrowth;
        public bool heardCharlottePlantInstructions;
    }
}