using System;
using UnityEngine;

namespace common.scripts
{
    [Serializable]
    public class FloorData
    {
        public int floorNumber;
    }

    [Serializable]
    public class FloorsData
    {
        public FloorData[] data;
    }

    public class JsonReader : MonoBehaviour
    {
        [SerializeField] private TextAsset jsonFile; // Drag your JSON file here in the editor

        private void Start()
        {
            LoadJson();
        }

        private void LoadJson()
        {
            if (jsonFile != null)
            {
                // Read the JSON content from the TextAsset
                var json = jsonFile.text;
                var data = JsonUtility.FromJson<FloorsData>(json).data;
                Debug.Log($"Number of floors: {data.Length}");
            }
            else
            {
                Debug.LogWarning("JSON file not attached!");
            }
        }
    }
}