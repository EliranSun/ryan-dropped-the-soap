using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ColliderOnMouseDown : MonoBehaviour {
    private void OnMouseDown() {
        PlayerChangeState.Instance.OnClick();
    }
}