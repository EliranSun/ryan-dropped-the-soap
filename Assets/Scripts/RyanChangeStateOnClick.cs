using UnityEngine;

public class RyanChangeStateOnClick : MonoBehaviour {
    [SerializeField] private State state;
    [SerializeField] private Sprite[] stateSprites;

    private void OnMouseDown() {
        state = state switch {
            State.Dressed => State.Shower,
            State.Shower => State.Dressed,
            _ => State.Dressed
        };
    }

    private enum State {
        Dressed,
        Shower,
        Drown,
        Dead
    }
}