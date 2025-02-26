using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class ShowerHead : MonoBehaviour
{
    [SerializeField] private Transform waterContainerParent;
    [SerializeField] private GameObject waterDrop;
    [SerializeField] private int dropletsPerInterval = 1;
    [SerializeField] private int waterDropCount;
    [SerializeField] private float waterDropInterval;
    [SerializeField] private float dropXForce = -50;
    [SerializeField] private float dropYForce;
    [SerializeField] private float xRange = 0.2f;
    private readonly List<GameObject> _waterPool = new();

    private void Awake()
    {
        for (var i = 0; i < waterDropCount; i++)
        {
            var tmp = Instantiate(waterDrop);
            tmp.SetActive(false);
            tmp.transform.parent = waterContainerParent;
            _waterPool.Add(tmp);
        }
    }

    private void Start()
    {
        StartCoroutine(DropWater());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // if (faucetLevel > 0 && waterDropInterval != GetWaterLevel())
        //     waterDropInterval = GetWaterLevel();
    }

    public GameObject GetPooledWaterDrop()
    {
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

    private IEnumerator DropWater()
    {
        while (GetPooledWaterDrop())
        {
            if (waterDropInterval is <= 0.01f or >= 1)
                yield return new WaitUntil(() => waterDropInterval is > 0.01f and < 1);

            yield return new WaitForSeconds(Random.Range(waterDropInterval, waterDropInterval + 0.1f));

            for (var i = 0; i < dropletsPerInterval; i++)
            {
                var newWaterDrop = GetPooledWaterDrop();

                if (!newWaterDrop)
                    yield break;

                ActivateWaterDrop(newWaterDrop);
                StartCoroutine(DisableWaterDrop(newWaterDrop));
            }
        }
    }

    private void ActivateWaterDrop(GameObject droplet)
    {
        var position = transform.position;
        position.x += Random.Range(-xRange, xRange);

        var dropletRigidBody = droplet.GetComponent<Rigidbody2D>();

        var randomXForce = Random.Range(dropXForce / 2, dropXForce);
        var randomYForce = Random.Range(dropYForce / 2, dropYForce);

        droplet.transform.position = position;
        droplet.SetActive(true);

        dropletRigidBody.AddForce(new Vector2(randomXForce, randomYForce));
        dropletRigidBody.gravityScale = Random.Range(1, 5);
    }

    private static IEnumerator DisableWaterDrop(GameObject droplet)
    {
        yield return new WaitForSeconds(1);
        droplet.SetActive(false);
    }

    public void OnNotify(GameEventData eventData)
    {
        switch (eventData.Name)
        {
            case GameEvents.FaucetClosing:
                waterDropInterval *= 2;
                break;

            case GameEvents.FaucetOpening:
                waterDropInterval /= 2;
                break;

            case GameEvents.None:
                break;
        }
    }
}