using UnityEngine;

namespace Object.Scripts
{
    public class PlayerPositionObjectWrap : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private GameObject[] children;

        private float _previousYPosition;
        private float _screenHeight;
        private float _yDirection;

        private void Start()
        {
            _screenHeight = mainCamera.orthographicSize * 2;
            // _screenHeight = Screen.height;
        }

        private void Update()
        {
            _yDirection = player.transform.position.y - _previousYPosition;
            _previousYPosition = player.transform.position.y;

            HandleObjectRepositioning();
        }

        private void HandleObjectRepositioning()
        {
            if (_yDirection > 0) // up
            {
                var highestChild = GetExtremeObject(children, true);
                var lowestChild = GetExtremeObject(children, false);

                print($"Moving up. Player Y: {player.transform.position.y}; Highest floor: {highestChild.name} at Y: {highestChild.transform.position.y}");

                // If player is above the highest object, wrap the lowest object above the player
                if (player.transform.position.y > highestChild.transform.position.y)
                    lowestChild.transform.position = new Vector3(
                        lowestChild.transform.position.x,
                        player.transform.position.y + _screenHeight,
                        lowestChild.transform.position.z
                    );
            }
            else if (_yDirection < 0) // down
            {
                var highestChild = GetExtremeObject(children, true);
                var lowestChild = GetExtremeObject(children, false);

                print($"Moving down. Player Y: {player.transform.position.y}; Lowest floor: {lowestChild.name} at Y: {lowestChild.transform.position.y}");

                // If player is below the lowest object, wrap the highest object below the player
                if (player.transform.position.y < lowestChild.transform.position.y)
                    highestChild.transform.position = new Vector3(
                        highestChild.transform.position.x,
                        player.transform.position.y - _screenHeight,
                        highestChild.transform.position.z
                    );
            }
        }

        private static GameObject GetExtremeObject(GameObject[] objects, bool highest)
        {
            var extreme = objects[0];
            foreach (var obj in objects)
                if (highest && obj.transform.position.y > extreme.transform.position.y)
                    extreme = obj;
                else if (!highest && obj.transform.position.y < extreme.transform.position.y)
                    extreme = obj;
            return extreme;
        }
    }
}