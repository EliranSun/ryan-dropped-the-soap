using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RopeAttachable : ObserverSubject
{
    [SerializeField] private GameObject ropeObject;
    private bool isPlayerHoldingRope;
    private bool isRopeUsed;

    private void OnMouseDown()
    {
        if (isRopeUsed)
        {
            isRopeUsed = false;
            ropeObject.SetActive(false);
            Notify(GameEvents.RopeDetached);
            return;
        }

        if (isPlayerHoldingRope && !isRopeUsed)
        {
            isRopeUsed = true;
            ropeObject.SetActive(true);
            Notify(GameEvents.RopeAttached);
        }
    }

    public void OnNotify(GameEventData eventData)
    {
        if (eventData.Name == GameEvents.RopeInHand)
        {
            isPlayerHoldingRope = true;
        }

        if (eventData.Name == GameEvents.RopeInBag)
        {
            isPlayerHoldingRope = false;
        }
    }
}
