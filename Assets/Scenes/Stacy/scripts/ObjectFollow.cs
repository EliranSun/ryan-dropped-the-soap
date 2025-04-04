using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFollow : MonoBehaviour
{
    [SerializeField] Transform objectToFollow;
    private Vector3 _initialPosition;

    private void Start()
    {
        _initialPosition = transform.position;
    }

    private void Update()
    {
        if (objectToFollow == null) return;

        var newPosition = objectToFollow.position;
        newPosition.z = transform.position.z;
        newPosition.x -= 5;
        newPosition.y += 3;

        transform.position = newPosition;
    }
}
