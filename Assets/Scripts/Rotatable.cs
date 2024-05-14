using UnityEngine;

public class Rotatable : MonoBehaviour {
    [SerializeField] private int sensitivity = 2;

    private void Update() {
        var scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
            transform.Rotate(new Vector3(0, 0, sensitivity));
        else if (scroll < 0f)
            transform.Rotate(new Vector3(0, 0, -sensitivity));
    }
}