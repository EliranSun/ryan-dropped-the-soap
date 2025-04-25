using System.Collections;
using System.Collections.Generic;
using Character.Scripts;
using UnityEngine;

public class PlayerDragBody : MonoBehaviour
{
    [SerializeField] GameObject theBody;
    Transform _playerTransform;
    Movement _movement;
    bool _isEnabled = false;

    void Start()
    {
        _playerTransform = GetComponent<Transform>();
        _movement = GetComponent<Movement>();
    }

    void Update()
    {
        if (!theBody) return;

        var distanceFromBody = Mathf.Abs(Vector2.Distance(theBody.transform.position, _playerTransform.position));

        if (Input.GetButton("Fire1") && distanceFromBody <= 2)
        {
            if (!_isEnabled)
            {
                _isEnabled = true;
                _movement.SlowDown();
                theBody.transform.rotation = Quaternion.Euler(0, 0, -90);
            }

            theBody.transform.position = new Vector3(_playerTransform.position.x + 1, _playerTransform.position.y - 1f, 0);
        }

        if (Input.GetButtonUp("Fire1"))
        {
            if (_isEnabled)
            {
                _isEnabled = false;
                _movement.NormalSpeed();
            }
        }
    }
}
