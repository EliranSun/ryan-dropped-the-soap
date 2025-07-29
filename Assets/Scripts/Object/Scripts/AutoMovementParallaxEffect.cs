using UnityEngine;

namespace Object.Scripts
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public class AutoMovementParallaxEffect : MonoBehaviour
    {
        [SerializeField] private float speed = 0.5f;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Direction direction;
        [SerializeField] private GameObject[] children;
        private float _screenHeight;

        private void Start()
        {
            _screenHeight = mainCamera.orthographicSize * 2;
        }

        private void Update()
        {
            var position = transform.position;
            switch (direction)
            {
                case Direction.Up:
                    position.y += Time.deltaTime * speed;
                    break;

                case Direction.Down:
                    position.y -= Time.deltaTime * speed;
                    break;

                case Direction.Left:
                    position.x -= Time.deltaTime * speed;
                    break;

                case Direction.Right:
                    position.x += Time.deltaTime * speed;
                    break;
            }

            HandleObjectRepositioning();
            transform.position = position;
        }

        private void HandleObjectRepositioning()
        {
            switch (direction)
            {
                case Direction.Up:
                    var highestChild = GetExtremeObject(children, true);
                    if (highestChild.transform.position.y > _screenHeight)
                        highestChild.transform.position = new Vector3(highestChild.transform.position.x,
                            -_screenHeight, highestChild.transform.position.z);
                    break;

                case Direction.Down:
                    var lowestChild = GetExtremeObject(children, false);
                    if (lowestChild.transform.position.y < -_screenHeight)
                        lowestChild.transform.position = new Vector3(lowestChild.transform.position.x,
                            _screenHeight, lowestChild.transform.position.z);
                    break;
            }
        }

        private GameObject GetExtremeObject(GameObject[] objects, bool highest)
        {
            var extreme = objects[0];
            foreach (var obj in objects)
            {
                if (highest && obj.transform.position.y > extreme.transform.position.y)
                    extreme = obj;
                else if (!highest && obj.transform.position.y < extreme.transform.position.y)
                    extreme = obj;
            }
            return extreme;
        }
    }
}