using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFollow : MonoBehaviour
{
    [SerializeField] Transform objectToFollow;
    [SerializeField] private float xOffset = 5;
    [SerializeField] private float yOffset = 3;

    private void Update()
    {
        if (objectToFollow == null) return;

        var newPosition = objectToFollow.position;
        newPosition.z = transform.position.z;
        newPosition.x -= xOffset;
        newPosition.y += yOffset;

        transform.position = newPosition;
    }
}
