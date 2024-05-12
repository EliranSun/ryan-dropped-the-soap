using System.Collections;
using Observer;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class ShowerHead : MonoBehaviour {
    [SerializeField] private Transform waterContainerParent;
    [SerializeField] private GameObject waterDrop;
    [SerializeField] private int waterDropCount;
    [SerializeField] private int faucetLevel = 1;
    [SerializeField] private float waterDropInterval;

    private void Start() {
        StartCoroutine(DropWater());
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        if (Input.GetKeyDown(KeyCode.Alpha0))
            faucetLevel = 0;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            faucetLevel = 1;

        if (Input.GetKeyDown(KeyCode.Alpha2))
            faucetLevel = 2;

        if (Input.GetKeyDown(KeyCode.Alpha3))
            faucetLevel = 3;

        if (faucetLevel > 0 && waterDropInterval != (float)1 / faucetLevel)
            waterDropInterval = (float)1 / faucetLevel;
    }

    private IEnumerator DropWater() {
        while (waterDropCount > 0) {
            if (faucetLevel == 0)
                yield return new WaitUntil(() => faucetLevel > 0);

            yield return new WaitForSeconds(waterDropInterval);
            var position = transform.position;
            position.y -= 0.5f;
            position.x += Random.Range(-0.2f, 0.2f);

            var newWater = Instantiate(waterDrop, position, Quaternion.identity);
            newWater.transform.parent = waterContainerParent;
            waterDropCount--;
        }
    }

    public void OnNotify(GameEvents eventName) {
        print(eventName);
    }
}