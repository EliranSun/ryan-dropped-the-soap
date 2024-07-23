using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextController : MonoBehaviour
{
    [SerializeField] private float hideAfter;
    private TextMeshProUGUI _text;
    private float _time;

    private void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _time = Time.time;
    }

    private void Update()
    {
        _time = Time.time;

        if (hideAfter > 0 && _time >= hideAfter)
            _text.gameObject.SetActive(false);
    }
}