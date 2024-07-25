using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextController : MonoBehaviour
{
    [SerializeField] private float showAfter;
    [SerializeField] private float hideAfter;
    private TextMeshProUGUI _text;
    private float _time;

    private void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _time = Time.time;

        if (showAfter > 0) _text.enabled = false;
    }

    private void Update()
    {
        _time = Time.time;

        if (showAfter > 0 && _time >= showAfter && !_text.enabled)
            _text.enabled = true;

        if (hideAfter > 0 && _time >= hideAfter && _text.enabled)
            _text.enabled = false;
    }
}