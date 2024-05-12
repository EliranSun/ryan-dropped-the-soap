using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class ShowerHead : MonoBehaviour {
    [SerializeField] private Transform waterContainerParent;
    [SerializeField] private GameObject waterDrop;
    [SerializeField] private int waterDropCount;
    [SerializeField] private int faucetLevel = 1;
    [SerializeField] private float waterDropInterval;
    private readonly List<GameObject> _waterPool = new();

    private void Awake() {
        for (var i = 0; i < waterDropCount; i++) {
            var tmp = Instantiate(waterDrop);
            tmp.SetActive(false);
            _waterPool.Add(tmp);
        }
    }

    private void Start() {
        StartCoroutine(DropWater());
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        if (faucetLevel > 0 && waterDropInterval != GetWaterLevel())
            waterDropInterval = GetWaterLevel();
    }

    public GameObject GetPooledWaterDrop() {
        foreach (var drop in _waterPool)
            if (!drop.activeInHierarchy)
                return drop;

        print("NEW");
        var obj = Instantiate(waterDrop);
        obj.SetActive(false);
        _waterPool.Add(obj);
        return obj;
    }

    private float GetWaterLevel() {
        return (float)1 / (faucetLevel * 10);
    }

    private IEnumerator DropWater() {
        while (waterDropCount > 0) {
            if (faucetLevel == 0)
                yield return new WaitUntil(() => faucetLevel > 0);

            yield return new WaitForSeconds(waterDropInterval);
            var position = transform.position;
            position.y -= 0.5f;
            position.x += Random.Range(-0.2f, 0.2f);

            var newWater = GetPooledWaterDrop();
            newWater.transform.parent = waterContainerParent;
            newWater.transform.position = position;
            newWater.SetActive(true);
            waterDropCount--;
        }
    }

    public void OnNotify(GameEvents eventName) {
        if (eventName == GameEvents.FaucetClosing)
            faucetLevel--;

        if (eventName == GameEvents.FaucetOpening)
            faucetLevel++;
    }
}