using System;
using System.Linq;
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

[RequireComponent(typeof(SpriteRenderer))]

public class RyanChangeStateOnClick : MonoBehaviour {
    [SerializeField] private State state;
    [SerializeField] private SpriteState[] spritesStates;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown() {
        state = state switch {
            State.Dressed => State.Shower,
            State.Shower => State.Dressed,
            _ => State.Dressed
        };

        _spriteRenderer.sprite = spritesStates.First(item => item.state == state).sprite;
    }


}