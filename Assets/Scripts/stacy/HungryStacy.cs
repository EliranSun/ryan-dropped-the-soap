using System.Collections;
using Dialog;
using UnityEngine;

public class HungryStacy : ObserverSubject
{
    [SerializeField] private GameObject imageContainer;
    [SerializeField] private NarrationDialogLine[] lines;
    [SerializeField] private float hungerInterval = 20f;
    [SerializeField] private GameEvents fedEvent = GameEvents.None;
    private int _dialogIndex;
    private bool _isFed;

    private void Start()
    {
        imageContainer.SetActive(false);
        StartCoroutine(GetHungry());
    }

    private IEnumerator GetHungry()
    {
        while (!_isFed)
        {
            if (lines.Length == 0) break;

            yield return new WaitForSeconds(hungerInterval);

            if (_dialogIndex >= lines.Length)
            {
                yield return new WaitForSeconds(12);
                imageContainer.SetActive(false);
                Notify(GameEvents.SandwichConsumed);
                _isFed = true;
                break;
            }

            if (_dialogIndex == lines.Length - 1)
            {
                imageContainer.SetActive(true);
                yield return new WaitForSeconds(2);
            }

            Notify(GameEvents.TriggerSpecificDialogLine, lines[_dialogIndex]);
            _dialogIndex++;
        }
    }

    public void OnNotify(GameEventData eventData)
    {
        if (eventData.Name == fedEvent)
        {
            _isFed = true;
            imageContainer.SetActive(false);
            StopAllCoroutines();
        }
    }
}