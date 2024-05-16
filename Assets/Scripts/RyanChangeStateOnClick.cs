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
public class SpriteState {
    public State state;
    public GameObject gameObjectPosition;
    public Sprite sprite;
}

[RequireComponent(typeof(SpriteRenderer))]
public class RyanChangeStateOnClick : MonoBehaviour {
    [SerializeField] private State state;
    [SerializeField] private SpriteState[] spritesStates;
    private SpriteRenderer _spriteRenderer;

    private void Start() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown() {
        state = state switch {
            State.Dressed => State.Shower,
            State.Shower => State.Dressed,
            _ => State.Dressed
        };

        var newSpriteState = spritesStates.First(item => item.state == state);

        _spriteRenderer.sprite = newSpriteState.sprite;

        if (newSpriteState.gameObjectPosition)
            transform.position = newSpriteState.gameObjectPosition.transform.position;
    }
}