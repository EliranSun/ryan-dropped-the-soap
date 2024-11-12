using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Character_Creator.scripts
{
    public class RotateObjectsAroundAxis : MonoBehaviour
    {
        [SerializeField] private List<GameObject> prefabsToRotate = new();
        [SerializeField] private float rotationSpeed = 30f; // degrees per second
        [SerializeField] private float radius = 2f; // Distance from center
        [SerializeField] private Vector2 centerPoint = Vector2.zero;
        [SerializeField] private DialogueManager dialogueManager;
        private readonly List<float> angles = new();
        private readonly List<GameObject> instances = new();

        private void Update()
        {
            for (var i = 0; i < instances.Count; i++)
                if (instances[i])
                {
                    angles[i] += rotationSpeed * Time.deltaTime;
                    var newPosition = centerPoint + (Vector2.right * radius).Rotate(angles[i]);
                    instances[i].transform.position = newPosition;
                }
        }

        private void OnDestroy()
        {
            foreach (var obj in instances.Where(obj => obj != null))
                Destroy(obj);
        }

        public void OnNotify(GameEventData gameEventData)
        {
            if (gameEventData.name == GameEvents.CharacterCreatorShowDoors) Init();
        }

        private void Init()
        {
            // Calculate even spacing for objects
            var angleStep = 360f / prefabsToRotate.Count;

            for (var i = 0; i < prefabsToRotate.Count; i++)
                if (prefabsToRotate[i] != null)
                {
                    var angle = i * angleStep;
                    var position = centerPoint + (Vector2.right * radius).Rotate(angle);

                    var instance = Instantiate(prefabsToRotate[i], position, Quaternion.identity);
                    instance.GetComponent<OnInteractableObject>().observers.AddListener(dialogueManager.OnNotify);
                    instances.Add(instance);
                    angles.Add(angle);
                }
        }
    }

    // Extension method to rotate a vector
    public static class Vector2Extension
    {
        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            var radians = degrees * Mathf.Deg2Rad;
            var sin = Mathf.Sin(radians);
            var cos = Mathf.Cos(radians);

            return new Vector2(
                v.x * cos - v.y * sin,
                v.x * sin + v.y * cos
            );
        }
    }
}