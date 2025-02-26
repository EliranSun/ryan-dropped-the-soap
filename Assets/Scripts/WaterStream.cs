using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SpriteRenderer))]
public class WaterStream : MonoBehaviour
{
    [SerializeField] private GameObject waterDrop;
    [SerializeField] private Transform waterStreamTransform;
    [SerializeField] private float waterStreamGrowth = 0.05f;
    [SerializeField] private float maxWidth = 0.6f;
    private int _faucetLevel;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnNotify(GameEventData eventData)
    {
        switch (eventData.Name)
        {
            case GameEvents.FaucetClosing:
            {
                _faucetLevel--;
                if (_faucetLevel == 0)
                {
                    StopAllCoroutines();
                    _spriteRenderer.enabled = false;
                    break;
                }

                ChangeWaterStream(true);
                StopCoroutine(Splash());
                break;
            }

            case GameEvents.FaucetOpening:
            {
                _faucetLevel++;
                if (waterStreamTransform.localScale.x >= maxWidth)
                    break;

                _spriteRenderer.enabled = true;
                ChangeWaterStream(false);
                StartCoroutine(Splash());
                break;
            }
        }
    }

    private void ChangeWaterStream(bool isSubtract)
    {
        var newScale = waterStreamTransform.localScale;
        newScale.x += isSubtract ? waterStreamGrowth * -1 : waterStreamGrowth;
        waterStreamTransform.localScale = newScale;
    }

    private IEnumerator Splash()
    {
        while (waterStreamTransform.localScale.x > 0)
        {
            yield return new WaitForSeconds(0.2f);

            var newDroplet = SpawnDroplet();
            RandomDropletForce(newDroplet.GetComponent<Rigidbody2D>());
        }
    }

    private void RandomDropletForce(Rigidbody2D dropletRigidBody)
    {
        var randomXForce = Random.Range(-5, 5);
        var randomYForce = Random.Range(3, 8);
        var randomForce = new Vector2(randomXForce, randomYForce);

        dropletRigidBody.AddForce(randomForce, ForceMode2D.Impulse);
    }

    private GameObject SpawnDroplet()
    {
        var position = transform.position;
        position.y -= transform.localScale.y / 2; // instantiate at the middle of the object
        var newDroplet = Instantiate(waterDrop, position, Quaternion.identity);
        newDroplet.transform.parent = transform;
        Destroy(newDroplet, 1);

        return newDroplet;
    }
}