using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    [Serializable]
    public class Item
    {
        public string name;
        public GameObject gameObject;
        public Vector3 position;
        public string activeInScene;
        public bool isHeldByPlayer;
    }

    public class ItemsManager : MonoBehaviour
    {
        [SerializeField] private Item[] items;
        [SerializeField] private GameObject player;
        public static ItemsManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            if (Instance == this) SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var currentSceneName = scene.name;
            foreach (var item in items)
                if (item.activeInScene == currentSceneName)
                {
                    var newItem = SpawnItem(item);
                    if (item.isHeldByPlayer) newItem.GetComponent<HoldableItem>().Hold();
                }
        }

        private GameObject SpawnItem(Item item)
        {
            var newItem = Instantiate(item.gameObject, item.position, Quaternion.identity);
            newItem.GetComponent<HoldableItem>().observers.AddListener(OnNotify);
            newItem.gameObject.name = item.name;
            return newItem;
        }

        public void OnNotify(GameEventData eventData)
        {
            if (eventData.Name == GameEvents.PickedItem)
            {
                var item = GetItem(eventData.Data.ToString());
                if (item != null) item.isHeldByPlayer = true;
            }

            if (eventData.Name == GameEvents.DroppedItem)
            {
                var item = GetItem(eventData.Data.ToString());
                if (item != null)
                {
                    item.isHeldByPlayer = false;
                    item.activeInScene = SceneManager.GetActiveScene().name;
                    item.position = player.transform.position;
                }
            }
        }

        private Item GetItem(string itemName)
        {
            return items.FirstOrDefault(item => item.name == itemName);
        }
    }
}