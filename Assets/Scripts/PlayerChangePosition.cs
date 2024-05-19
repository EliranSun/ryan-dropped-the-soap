using System;
using System.Linq;
using UnityEngine;

public enum PositionName {
    Default,
    OutOfShower,
    InShower
}

[Serializable]
public class Position {
    public PositionName name;
    public GameObject position;
}


public class PlayerChangePosition : ObserverSubject {
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Position[] positions;
    private PositionName _currentPositionName;

    private void Start() {
        _currentPositionName = positions[0].name;
        Notify(GameEvents.OutOfShower);
    }

    private void OnMouseDown() {
        var nextPositionName = _currentPositionName switch {
            PositionName.OutOfShower => PositionName.InShower,
            PositionName.InShower => PositionName.OutOfShower,
            _ => PositionName.OutOfShower
        };

        Notify(nextPositionName == PositionName.InShower
            ? GameEvents.InShower
            : GameEvents.OutOfShower);

        GameState.IsPlayerInShower = nextPositionName == PositionName.InShower;

        playerTransform.position = positions.First(item => item.name == nextPositionName).position.transform.position;
        _currentPositionName = nextPositionName;
    }
}