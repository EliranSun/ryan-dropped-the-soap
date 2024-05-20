using System.Collections;
using UnityEngine;

public class WaterStream : MonoBehaviour {
    [SerializeField] private GameObject waterDrop;
    [SerializeField] private Transform waterStreamTransform;
    [SerializeField] private float waterStreamGrowth = 0.05f;
    [SerializeField] private float maxWidth = 0.6f;

    public void OnNotify(GameEventData eventData) {
        switch (eventData.name) {
            case GameEvents.FaucetClosing: {
                if (waterStreamTransform.localScale.x <= 0) {
                    StopAllCoroutines();
                    break;
                }

                ChangeWaterStream(true);
                break;
            }

            case GameEvents.FaucetOpening: {
                if (waterStreamTransform.localScale.x >= maxWidth)
                    break;

                ChangeWaterStream(false);
                StartCoroutine(Splash());
                break;
            }
        }
    }

    private void ChangeWaterStream(bool isSubtract) {
        var newScale = waterStreamTransform.localScale;
        newScale.x += isSubtract ? waterStreamGrowth * -1 : waterStreamGrowth;
        waterStreamTransform.localScale = newScale;
    }

    private IEnumerator Splash() {
        while (waterStreamTransform.localScale.x > 0) {
            yield return new WaitForSeconds(1f);

            var position = transform.position;
            position.y -= transform.localScale.y / 2; // instantiate at the middle of the object
            var newWaterDrop = Instantiate(waterDrop, position, Quaternion.identity);

            newWaterDrop.transform.parent = transform;
            var direction = new Vector2(1, 5);
            newWaterDrop.GetComponent<Rigidbody2D>().AddForce(direction, ForceMode2D.Impulse);
            Destroy(newWaterDrop, 1);
        }
    }
}