using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Object.Scripts
{
    public class CoinsController : MonoBehaviour
    {
        [SerializeField] private GameObject coin;
        [SerializeField] private float spawnRate = 1;
        [SerializeField] private int boundsX = 4;
        [SerializeField] private float boundsY = 6f;

        private void Start()
        {
            StartCoroutine(SpawnCoins());
        }


        private IEnumerator SpawnCoins()
        {
            // create a list of coins positions out of boundsX and randomaly choose one of them
            var coinsPositions = new List<int>();

            for (var i = -boundsX; i <= boundsX; i++)
                coinsPositions.Add(i);

            while (true)
            {
                var randomX = coinsPositions[Random.Range(0, coinsPositions.Count)];
                Instantiate(coin, new Vector3(randomX, boundsY, 0), Quaternion.identity);
                yield return new WaitForSeconds(spawnRate);
            }
        }
    }
}