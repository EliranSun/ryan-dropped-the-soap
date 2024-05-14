using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class ShowerHead : MonoBehaviour {
    [SerializeField] private Transform waterContainerParent;
    [SerializeField] private GameObject waterDrop;
    [SerializeField] private int waterDropCount;
    [SerializeField] private float waterDropInterval;
    private readonly List<GameObject> _waterPool = new();

    private void Awake() {
        for (var i = 0; i < waterDropCount; i++) {
            var tmp = Instantiate(waterDrop);
            tmp.SetActive(false);
            tmp.transform.parent = waterContainerParent;
            _waterPool.Add(tmp);
        }
    }

    private void Start() {
        StartCoroutine(DropWater());
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // if (faucetLevel > 0 && waterDropInterval != GetWaterLevel())
        //     waterDropInterval = GetWaterLevel();
    }

    public GameObject GetPooledWaterDrop() {
        foreach (var drop in _waterPool)
            if (!drop.activeInHierarchy)
                return drop;

        // var obj = Instantiate(waterDrop);
        // obj.SetActive(false);
        // _waterPool.Add(obj);
        // return obj;
        return null;
    }

    // private float GetWaterLevel() {
    //     return (float)1 / (faucetLevel * 10);
    // }

    private IEnumerator DropWater() {
        while (GetPooledWaterDrop()) {
            if (waterDropInterval is <= 0.01f or >= 1)
                yield return new WaitUntil(() => waterDropInterval is > 0.01f and < 1);


            for (var i = 0; i < 20; i++) {
                yield return new WaitForSeconds(Random.Range(waterDropInterval, waterDropInterval + 0.1f));

                var newWaterDrop = GetPooledWaterDrop();

                if (!newWaterDrop)
                    yield break;

                var position = transform.position;
                position.y -= 0.1f;

                position.x += Random.Range(-0.2f, 0.2f);
                newWaterDrop.transform.position = position;
                newWaterDrop.SetActive(true);
                newWaterDrop.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(50, -50), 1));
                StartCoroutine(DisableWaterDrop(newWaterDrop));
            }
        }
    }

    private IEnumerator DisableWaterDrop(GameObject waterDrop) {
        yield return new WaitForSeconds(1);
        waterDrop.SetActive(false);
    }

    public void OnNotify(GameEvents eventName) {
        switch (eventName) {
            case GameEvents.FaucetClosing:
                waterDropInterval *= 2;
                break;

            case GameEvents.FaucetOpening:
                waterDropInterval /= 2;
                break;

            case GameEvents.None:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(eventName), eventName, null);
        }
    }
}