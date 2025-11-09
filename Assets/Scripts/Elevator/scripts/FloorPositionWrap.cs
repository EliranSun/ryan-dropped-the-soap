using UnityEngine;
using UnityEngine.Serialization;

namespace Elevator.scripts
{
    public class FloorPositionWrap : MonoBehaviour
    {
        [FormerlySerializedAs("target")] [SerializeField]
        private GameObject player;

        [SerializeField] private Camera mainCamera;
        [SerializeField] private float childHeight = 9.6f;
        [SerializeField] private float stopWrapBelowY = 54f;
        [SerializeField] private GameObject[] children;

        private float _previousYPosition;
        private float _yDirection;

        private void Start()
        {
            for (var i = 0; i < children.Length; i++)
                children[i].GetComponent<FloorWrapController>().UpdateFloorNumber(i + 1);
        }

        private void Update()
        {
            var targetYPosition = player.transform.position.y;
            _yDirection = targetYPosition - _previousYPosition;

            // Detect large position jumps (teleports)
            if (Mathf.Abs(_yDirection) > childHeight * 2) RepositionFloorsAroundTarget(targetYPosition);

            _previousYPosition = targetYPosition;

            if (targetYPosition < stopWrapBelowY)
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

        private void RepositionFloorsAroundTarget(float targetY)
        {
            // Calculate the floor number at target position
            var baseFloorNumber = Mathf.RoundToInt(targetY / childHeight);

            // Distribute floors around the target
            var floorsBelow = children.Length / 2;

            for (var i = 0; i < children.Length; i++)
            {
                var floorOffset = i - floorsBelow;
                var newY = (baseFloorNumber + floorOffset) * childHeight;
                var newFloorNumber = baseFloorNumber + floorOffset;

                children[i].transform.position = new Vector3(
                    children[i].transform.position.x,
                    newY,
                    children[i].transform.position.z
                );

                children[i].GetComponent<FloorWrapController>().UpdateFloorNumber(newFloorNumber);
            }
        }

        private void WrapObject(GameObject child, float y)
        {
            var x = child.transform.position.x;
            var z = child.transform.position.z;

            print($"Wrapped object {child.gameObject.name} to {y}");

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

            child.GetComponent<FloorWrapController>().UpdateFloorNumber(newFloorNumber);
        }

        private int GetHighestFloorNumber()
        {
            var highest = 0;
            foreach (var child in children)
            {
                var controller = child.GetComponent<FloorWrapController>();
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
                var controller = child.GetComponent<FloorWrapController>();
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