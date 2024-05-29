using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ChangeStateMouseDown : MonoBehaviour {
    private void OnMouseDown() {
        PlayerChangeState.Instance.OnClick();
    }
}