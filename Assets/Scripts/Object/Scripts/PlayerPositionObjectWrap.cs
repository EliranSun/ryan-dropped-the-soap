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

        private void Start()
        {
            for (var i = 0; i < children.Length; i++)
                children[i].GetComponent<ObjectIndexController>().UpdateFloorNumber(i + 1);
        }

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
            var x = child.transform.position.x;
            var z = child.transform.position.z;
            child.transform.position = new Vector3(x, y, z);

            // Calculate the correct floor number based on wrapping direction
            int newFloorNumber;
            if (_yDirection > 0) // Moving up: object goes above highest, gets highest floor + 1
            {
                var highestFloor = GetHighestFloorNumber();
                newFloorNumber = highestFloor + 1;
            }
            else // Moving down: object goes below lowest, gets lowest floor - 1
            {
                var lowestFloor = GetLowestFloorNumber();
                newFloorNumber = lowestFloor - 1;
            }

            child.GetComponent<ObjectIndexController>().UpdateFloorNumber(newFloorNumber);
        }

        private int GetHighestFloorNumber()
        {
            var highest = 0;
            foreach (var child in children)
            {
                var controller = child.GetComponent<ObjectIndexController>();
                if (controller != null)
                {
                    var floorText = controller.GetFloorNumberText();
                    if (int.TryParse(floorText, out var floorNumber) && floorNumber > highest)
                        highest = floorNumber;
                }
            }
            return highest;
        }

        private int GetLowestFloorNumber()
        {
            var lowest = int.MaxValue;
            foreach (var child in children)
            {
                var controller = child.GetComponent<ObjectIndexController>();
                if (controller != null)
                {
                    var floorText = controller.GetFloorNumberText();
                    if (int.TryParse(floorText, out var floorNumber) && floorNumber < lowest)
                        lowest = floorNumber;
                }
            }
            return lowest == int.MaxValue ? 0 : lowest;
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