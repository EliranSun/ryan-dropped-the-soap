using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class ChangePlayerPosition : MonoBehaviour {
    [SerializeField] private Transform playerTransform;

    private void Start() {
        EventManager.Instance.Publish(GameEvents.OutOfShower);
    }

    private void OnMouseDown() {
        if (CursorManager.Instance.IsActionCursor)
            return;

        playerTransform.position = transform.position;
        var playerPositions = GameObject.FindGameObjectsWithTag("PlayerPosition");

        foreach (var playerPosition in playerPositions) {
            var isSelf = playerPosition.name == name;
            playerPosition.GetComponent<Collider2D>().enabled = !isSelf;
            playerPosition.GetComponent<SpriteRenderer>().enabled = !isSelf;

            if (isSelf)
                DisableChildren(playerPosition);
            else
                EnableChildren(playerPosition);
        }
    }

    private void EnableChildren(GameObject parent) {
        foreach (Transform child in parent.transform) child.gameObject.SetActive(true);
    }

    // Method to disable all children
    private void DisableChildren(GameObject parent) {
        foreach (Transform child in parent.transform) child.gameObject.SetActive(false);
    }
}