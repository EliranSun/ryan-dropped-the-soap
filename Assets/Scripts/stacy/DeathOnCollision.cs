using Dialog;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DeathOnCollision : ObserverSubject
{
    [SerializeField] private bool isEnabled = true;
    [SerializeField] private NarrationDialogLine[] lines;
    private int _linesIndex;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isEnabled) return;

        var collisionSpeed = collision.relativeVelocity.magnitude;

        if (collisionSpeed <= 50) return;

        gameObject.SetActive(false);
        Invoke(nameof(TriggerDeath), 3f);
    }

    private void TriggerDeath()
    {
        Notify(GameEvents.Dead);

        if (_linesIndex < lines.Length && lines.Length > 0)
        {
            Notify(GameEvents.TriggerSpecificDialogLine, lines[_linesIndex]);
            _linesIndex++;
        }
    }
}