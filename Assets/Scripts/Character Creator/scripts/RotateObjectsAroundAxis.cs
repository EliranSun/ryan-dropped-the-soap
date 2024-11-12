using System;
using System.Collections.Generic;
using UnityEngine;
using Enumerable = System.Linq.Enumerable;

namespace Character_Creator.scripts
{
    [Serializable]
    public class CharacterCreatorObjects
    {
        public List<GameObject> prefabs;
    }

    public class RotateObjectsAroundAxis : MonoBehaviour
    {
        [SerializeField] private CharacterCreatorObjects[] prefabsToRotate;
        [SerializeField] private float rotationSpeed = 30f; // degrees per second
        [SerializeField] private float radius = 2f; // Distance from center
        [SerializeField] private Vector2 centerPoint = Vector2.zero;
        [SerializeField] private DialogueManager dialogueManager;
        private readonly List<float> angles = new();
        private readonly List<GameObject> instances = new();
        private int _activePrefabsIndex;

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
            foreach (var obj in Enumerable.Where(instances, obj => obj != null))
                Destroy(obj);
        }

        public void OnNotify(GameEventData gameEventData)
        {
            if (gameEventData.name == GameEvents.CharacterCreatorNextSetOfObjects)
            {
                Destroy();
                Init();
                _activePrefabsIndex++;
            }

            if (gameEventData.name == GameEvents.CharacterCreatorHideObjects)
                Destroy();
        }

        private void Init()
        {
            // Calculate even spacing for objects
            var activePrefabs = prefabsToRotate[_activePrefabsIndex];
            var angleStep = 360f / activePrefabs.prefabs.Count;

            for (var i = 0; i < activePrefabs.prefabs.Count; i++)
                if (activePrefabs.prefabs[i] != null)
                {
                    var angle = i * angleStep;
                    var position = centerPoint + (Vector2.right * radius).Rotate(angle);

                    var instance = Instantiate(activePrefabs.prefabs[i], position, Quaternion.identity);
                    instance.GetComponent<OnInteractableObject>().observers.AddListener(dialogueManager.OnNotify);
                    instances.Add(instance);
                    angles.Add(angle);
                }
        }

        private void Destroy()
        {
            foreach (var obj in Enumerable.Where(instances, obj => obj != null))
                Destroy(obj);
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