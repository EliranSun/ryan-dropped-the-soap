using System;
using UnityEngine;

public enum State {
    None,
    Dressed,
    Shower,
    Drown,
    Dead
}

[Serializable]
public class SpriteState
{
    public Sprite sprite;
    public State state;
}

public class RyanChangeStateOnClick : MonoBehaviour {
    [SerializeField] private State state;
    [SerializeField] private SpriteState[] spritesStates;

    private void OnMouseDown() {
        state = state switch {
            State.Dressed => State.Shower,
            State.Shower => State.Dressed,
            _ => State.Dressed
        };
    }


}