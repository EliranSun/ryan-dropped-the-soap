using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextController : MonoBehaviour
{
    [SerializeField] private float showAfter;
    [SerializeField] private float hideAfter;
    private float _initTime;
    private TextMeshProUGUI _text;
    private float _time;

    private void Start()
    {
        print(Time.time);

        _text = GetComponent<TextMeshProUGUI>();
        _initTime = Time.time;

        if (showAfter > 0) _text.enabled = false;
    }

    private void Update()
    {
        _time = Time.time;

        if (showAfter > 0 && _time >= showAfter + _initTime && !_text.enabled)
            _text.enabled = true;

        if (hideAfter > 0 && _time >= hideAfter + _initTime && _text.enabled)
            _text.enabled = false;
    }
}