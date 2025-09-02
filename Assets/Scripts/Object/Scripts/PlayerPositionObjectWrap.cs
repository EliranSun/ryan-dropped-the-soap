using UnityEngine;

namespace Object.Scripts
{
    public class PlayerPositionObjectWrap : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float childHeight = 9.6f;
        [SerializeField] private float minY = 54f;

        [SerializeField] private GameObject[] children;

        private float _previousYPosition;
        private float _yDirection;

        private void Update()
        {
            _yDirection = player.transform.position.y - _previousYPosition;
            _previousYPosition = player.transform.position.y;

            if (player.transform.position.y < minY)
                return;

            HandleObjectRepositioning();
        }

        private void HandleObjectRepositioning()
        {
            var highestChild = GetExtremeObject(children, true);
            var lowestChild = GetExtremeObject(children, false);

            if (_yDirection > 0) // up
            {
                // Check if camera's bottom edge passed the top of the lowest child
                var cameraBottomEdge = mainCamera.transform.position.y - mainCamera.orthographicSize;
                if (cameraBottomEdge > lowestChild.transform.position.y + childHeight)
                    WrapObject(lowestChild, highestChild.transform.position.y + childHeight);
            }
            else if (_yDirection < 0) // down
            {
                // Check if camera's top edge passed the bottom of the highest child
                var cameraTopEdge = mainCamera.transform.position.y + mainCamera.orthographicSize;
                if (cameraTopEdge < highestChild.transform.position.y - childHeight)
                    WrapObject(highestChild, lowestChild.transform.position.y - childHeight);
            }
        }

        private void WrapObject(GameObject child, float y)
        {
            child.transform.position = new Vector3(
                child.transform.position.x,
                y,
                child.transform.position.z
            );
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