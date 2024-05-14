using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class ShowerHead : MonoBehaviour {
    [SerializeField] private Transform waterContainerParent;
    [SerializeField] private GameObject waterDrop;
    [SerializeField] private int dropletsPerInterval = 1;
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

            yield return new WaitForSeconds(Random.Range(waterDropInterval, waterDropInterval + 0.1f));

            for (var i = 0; i < dropletsPerInterval; i++) {
                var newWaterDrop = GetPooledWaterDrop();

                if (!newWaterDrop)
                    yield break;

                ActivateWaterDrop(newWaterDrop);
                StartCoroutine(DisableWaterDrop(newWaterDrop));
            }
        }
    }

    private void ActivateWaterDrop(GameObject droplet) {
        var position = transform.position;
        position.x += Random.Range(-0.2f, 0.2f);

        var dropletRigidBody = droplet.GetComponent<Rigidbody2D>();
        dropletRigidBody.AddForce(new Vector2(Random.Range(50, -50), Random.Range(10, -10)));
        dropletRigidBody.gravityScale = Random.Range(2, 5);
        droplet.transform.position = position;
        droplet.SetActive(true);
    }

    private IEnumerator DisableWaterDrop(GameObject droplet) {
        yield return new WaitForSeconds(1);
        droplet.SetActive(false);
    }

    public void OnNotify(GameEventData eventData) {
        switch (eventData.name) {
            case GameEvents.FaucetClosing:
                waterDropInterval *= 2;
                break;

            case GameEvents.FaucetOpening:
                waterDropInterval /= 2;
                break;

            case GameEvents.None:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(eventData.name), eventData.name, null);
        }
    }
}