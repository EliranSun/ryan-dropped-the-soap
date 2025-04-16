using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(Light2DBase))]
public class ToggleLight : MonoBehaviour
{
    [SerializeField] private KeyCode toggleKey = KeyCode.F;
    private Light2DBase _light;

    void Start()
    {
        _light = GetComponent<Light2DBase>();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            _light.enabled = !_light.enabled;
        }
    }
}
