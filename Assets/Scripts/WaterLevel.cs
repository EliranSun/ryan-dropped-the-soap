using UnityEngine;

public class WaterLevel : MonoBehaviour {
    private int _waterThrottle;

    private void Update() {
        if (_waterThrottle == 0) return;

        transform.Translate(new Vector2(0, (float)1 / _waterThrottle * Time.deltaTime));
    }

    public void OnNotify(GameEvents eventName) {
        if (eventName == GameEvents.FaucetOpening) _waterThrottle -= 10;
        if (eventName == GameEvents.FaucetOpening) _waterThrottle += 10;
    }
}