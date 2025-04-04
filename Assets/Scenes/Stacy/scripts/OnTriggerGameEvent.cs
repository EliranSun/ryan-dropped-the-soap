using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerGameEvent : ObserverSubject
{
    [SerializeField] private string tagToDetect;
    [SerializeField] private GameEvents gameEventName;
    [SerializeField] private int delayEventTrigger;
    [SerializeField] private bool invokeOnce = true;
    private bool isInvoked;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (invokeOnce && isInvoked) return;

        if (collision.gameObject.CompareTag(tagToDetect))
        {
            isInvoked = true;
            Invoke(nameof(NotifyEvent), delayEventTrigger);
        }
    }

    private void NotifyEvent()
    {
        Notify(gameEventName);
    }
}
