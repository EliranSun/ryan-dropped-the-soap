using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Dialog.Scripts;

public class ZekeShoutsController : ObserverSubject
{
    [SerializeField] private Image zekeShoutsImage;
    [SerializeField] private Sprite[] zekeShoutsSprites;
    [SerializeField] private ThoughtChoice[] thoughtsChoice;


    public void Init()
    {
        StartCoroutine(ThinkRandomThought());
    }

    public IEnumerator ThinkRandomThought()
    {
        while (true)
        {
            zekeShoutsImage.sprite = zekeShoutsSprites[0];
            var randomIndex = Random.Range(0, thoughtsChoice.Length);
            var selectedThought = thoughtsChoice[randomIndex];

            Notify(GameEvents.AddThoughts, selectedThought);

            yield return new WaitForSeconds(1f);
        }
    }
}
