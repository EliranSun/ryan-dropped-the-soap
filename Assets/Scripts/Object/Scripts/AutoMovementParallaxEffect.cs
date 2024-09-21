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
        [SerializeField] private Direction direction;
        [SerializeField] private GameObject[] children;
        private float _screenHeight;

        private void Start()
        {
            _screenHeight = Camera.main.orthographicSize * 2;
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

            var highestChild = GetHighestObject(children);
            if (highestChild.transform.position.y > _screenHeight + 1)
                highestChild.transform.position = new Vector3(highestChild.transform.position.x,
                    -_screenHeight - 2, highestChild.transform.position.z);

            transform.position = position;
        }

        private GameObject GetHighestObject(GameObject[] objects)
        {
            var highest = objects[0];
            foreach (var obj in objects)
                if (obj.transform.position.y > highest.transform.position.y)
                    highest = obj;

            return highest;
        }
    }
}